namespace Tournaments.Test.Controllers;

public class TournamentsControllerTests
{
    private readonly TournamentsController _tournamentsController;
    private readonly Mock<ILogger<TournamentsController>> _mockLogger = new();
    private readonly IMapper _mapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly List<Tournament> _mockTournaments = TournamentFactory.Generate(2);

    public TournamentsControllerTests()
    {
        var config = new MapperConfiguration(config =>
        {
            config.AddProfile<TournamentMappingProfile>();
            config.AddProfile<GameMappingProfile>();
        });

        _mapper = config.CreateMapper();

        _tournamentsController = new TournamentsController(
            _mockLogger.Object,
            _mapper,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetTournaments_Returns_OkObjectResult_When_Entities_Exist()
    {
        // Arrange
        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.GetAllAsync())
            .ReturnsAsync(_mockTournaments);

        // Act
        var response = await _tournamentsController.GetTournaments();

        // Assert
        Assert.IsType<OkObjectResult>(response.Result);
    }

    [Fact]
    public async Task GetTournaments_Returns_All_Entities_When_Entities_Exist()
    {
        // Arrange
        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.GetAllAsync())
            .ReturnsAsync(_mockTournaments);

        // Act
        var result = await _tournamentsController.GetTournaments();
        var okResult = (OkObjectResult)result.Result!;
        var responseItems = (IEnumerable<TournamentAPIModel>)okResult.Value!;

        // Assert
        Assert.Equal(_mockTournaments.Count, responseItems.Count());
    }

    [Fact]
    public async Task GetTournaments_Returns_NoContentResult_When_No_Entities_Exist()
    {
        // Arrange
        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        var response = await _tournamentsController.GetTournaments();

        // Assert
        Assert.IsType<NoContentResult>(response.Result);
    }

    [Fact]
    public async Task GetTournamentById_Returns_OkObjectResult_When_Entity_Exists()
    {
        // Arrange
        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.GetAsync(1))
            .ReturnsAsync(_mockTournaments[0]);

        // Act
        var response = await _tournamentsController.GetTournamentById(1);

        // Assert
        Assert.IsType<OkObjectResult>(response.Result);
    }

    [Fact]
    public async Task GetTournamentById_Returns_Entity_When_Entity_Exists()
    {
        // Arrange
        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.GetAsync(1))
            .ReturnsAsync(_mockTournaments[0]);

        // Act
        var result = await _tournamentsController.GetTournamentById(1);
        var okResult = (OkObjectResult)result.Result!;
        var responseItem = (TournamentAPIModel)okResult.Value!;

        // Assert
        Assert.Equal(responseItem.Id, _mockTournaments[0].Id);
    }

    [Fact]
    public async Task GetTournamentById_Returns_NotFoundResult_When_Entity_Does_Not_Exist()
    {
        // Arrange
        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.GetAsync(3))
            .ReturnsAsync(() => null);

        // Act
        var response = await _tournamentsController.GetTournamentById(3);

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task CreateTournament_Returns_OkObjectResult_If_Entity_Is_Created()
    {
        // Arrange
        TournamentCreateAPIModel createModel = TournamentCreateAPIModelFactory.GenerateSingle();
        Tournament createdTournament = TournamentFactory.GenerateSingle();

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.AddAsync(createdTournament))
            .ReturnsAsync(createdTournament);

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository
            .AnyAsync(It.IsAny<Expression<Func<Tournament, bool>>>()))
            .ReturnsAsync((Expression<Func<Tournament, bool>> predicate) =>
                predicate.Compile().Invoke(createdTournament));

        // Act
        var response = await _tournamentsController.CreateTournament(createModel);

        // Assert
        Assert.IsType<OkObjectResult>(response.Result);
    }

    [Fact]
    public async Task PutTournament_Returns_OkObjectResult_If_Entity_Is_Updated()
    {
        // Arrange
        TournamentEditAPIModel editModel = TournamentEditAPIModelFactory.GenerateSingle();
        Tournament tournamentToEdit = TournamentFactory.GenerateSingle();

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository
            .AnyAsync(It.IsAny<Expression<Func<Tournament, bool>>>()))
            .ReturnsAsync((Expression<Func<Tournament, bool>> predicate) =>
                predicate.Compile().Invoke(tournamentToEdit));

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.UpdateAsync(tournamentToEdit))
            .ReturnsAsync(tournamentToEdit);

        // Act
        var response = await _tournamentsController.PutTournament(editModel);

        // Assert
        Assert.IsType<OkObjectResult>(response.Result);
    }

    [Fact]
    public async Task PutTournament_Returns_NotFoundResult_If_Entity_Does_Not_Exist()
    {
        // Arrange
        TournamentEditAPIModel editModel = TournamentEditAPIModelFactory.GenerateSingle();
        Tournament tournamentToEdit = TournamentFactory.GenerateSingle();

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository
           .AnyAsync(It.IsAny<Expression<Func<Tournament, bool>>>()))
           .ReturnsAsync((Expression<Func<Tournament, bool>> predicate) => false);

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.UpdateAsync(tournamentToEdit))
            .ReturnsAsync(tournamentToEdit);

        // Act
        var response = await _tournamentsController.PutTournament(editModel);

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task PatchTournament_Returns_OkObjectResult_If_Entity_Is_Patched()
    {
        // Arrange
        Tournament tournamentToPatch = TournamentFactory.GenerateSingle();

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.GetAsync(1))
            .ReturnsAsync(tournamentToPatch);

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository
            .AnyAsync(It.IsAny<Expression<Func<Tournament, bool>>>()))
            .ReturnsAsync((Expression<Func<Tournament, bool>> predicate) =>
                predicate.Compile().Invoke(tournamentToPatch));

        JsonPatchDocument<Tournament> patchDocument = JsonPatchDocumentFactory
            .GenerateTournamentPatchDocument();

        // Act
        var response = await _tournamentsController.PatchTournament(1, patchDocument);

        // Assert
        Assert.IsType<OkObjectResult>(response.Result);
    }

    [Fact]
    public async Task PatchTournament_Returns_NotFoundResult_If_Entity_Is_Does_Not_Exist()
    {
        // Arrange
        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.GetAsync(1))
            .ReturnsAsync(() => null);

        var tournamentToPatch = TournamentFactory.GenerateSingle();

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository
            .AnyAsync(It.IsAny<Expression<Func<Tournament, bool>>>()))
            .ReturnsAsync((Expression<Func<Tournament, bool>> predicate) =>
                predicate.Compile().Invoke(tournamentToPatch));

        JsonPatchDocument<Tournament> patchDocument = JsonPatchDocumentFactory
           .GenerateTournamentPatchDocument();

        // Act
        var response = await _tournamentsController.PatchTournament(1, patchDocument);

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task DeleteTournament_Returns_OkObjectResult_If_Entity_Is_Deleted()
    {
        // Arrange
        Tournament deletedTournament = TournamentFactory.GenerateSingle();

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository
            .AnyAsync(It.IsAny<Expression<Func<Tournament, bool>>>()))
            .ReturnsAsync((Expression<Func<Tournament, bool>> predicate) =>
                predicate.Compile().Invoke(deletedTournament));

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository.RemoveAsync(1))
            .ReturnsAsync(deletedTournament);

        // Act
        var response = await _tournamentsController.DeleteTournament(1);

        // Assert
        Assert.IsType<OkObjectResult>(response.Result);
    }

    [Fact]
    public async Task DeleteTournament_Returns_NotFoundResult_If_Entity_Does_Not_Exist()
    {
        // Arrange
        Tournament deletedTournament = TournamentFactory.GenerateSingle();

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository
            .AnyAsync(It.IsAny<Expression<Func<Tournament, bool>>>()))
            .ReturnsAsync((Expression<Func<Tournament, bool>> predicate) => false);

        _mockUnitOfWork.Setup(uow => uow.TournamentRepository
            .RemoveAsync(deletedTournament.Id))
            .ReturnsAsync(deletedTournament);

        // Act
        var response = await _tournamentsController.DeleteTournament(1);

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }
}