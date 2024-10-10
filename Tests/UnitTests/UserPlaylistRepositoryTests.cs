using Moq;
using Microsoft.Azure.Cosmos;
using MoviePlaylist.Repositories;
using Models;

namespace MoviePlaylist.UnitTests.Repositories
{
    [TestClass]
    public class UserPlaylistRepositoryTests
    {
        private Mock<CosmosClient> _mockCosmosClient;
        private Mock<Container> _mockContainer;
        private UserPlaylistRepository _userPlaylistRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockCosmosClient = new Mock<CosmosClient>();
            _mockContainer = new Mock<Container>();

            // Setup the mock CosmosClient to return the mock container
            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                             .Returns(_mockContainer.Object);

            _userPlaylistRepository = new UserPlaylistRepository(_mockCosmosClient.Object, "testDatabase", "testContainer");
        }

        [TestMethod]
        public async Task AddPlaylistAsync_ShouldCallUpsertItemAsync()
        {
            // Arrange
            var userPlaylist = new UserCurrentPlaylist
            {
                UserId = "user123",
                PlaylistId = "playlist123",
                LastStoppedAt = DateTime.UtcNow
            };

            // Act
            await _userPlaylistRepository.AddPlaylistAsync(userPlaylist);

            // Assert
            _mockContainer.Verify(x => x.UpsertItemAsync(userPlaylist, It.IsAny<PartitionKey>(), null, default), Times.Once);
        }

        [TestMethod]
        public async Task SaveUserPlaylistAsync_ShouldCallUpsertItemAsync()
        {
            // Arrange
            var userPlaylist = new UserCurrentPlaylist
            {
                UserId = "user123",
                PlaylistId = "playlist123",
                LastStoppedAt = DateTime.UtcNow
            };

            // Act
            await _userPlaylistRepository.SaveUserPlaylistAsync(userPlaylist);

            // Assert
            _mockContainer.Verify(x => x.UpsertItemAsync(userPlaylist, It.IsAny<PartitionKey>(), null, default), Times.Once);
        }

        [TestMethod]
        public async Task GetUserPlaylistAsync_ShouldReturnPlaylist_WhenFound()
        {
            // Arrange
            var expectedPlaylist = new UserCurrentPlaylist
            {
                UserId = "user123",
                PlaylistId = "playlist123"
            };

            var response = new Mock<ItemResponse<UserCurrentPlaylist>>();
            response.Setup(x => x.Resource).Returns(expectedPlaylist);

            _mockContainer.Setup(x => x.ReadItemAsync<UserCurrentPlaylist>(It.IsAny<string>(), It.IsAny<PartitionKey>(), null, default))
                          .ReturnsAsync(response.Object);

            // Act
            var result = await _userPlaylistRepository.GetUserPlaylistAsync("user123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedPlaylist.UserId, result.UserId);
            Assert.AreEqual(expectedPlaylist.PlaylistId, result.PlaylistId);
        }

        [TestMethod]
        public async Task GetUserPlaylistAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _mockContainer.Setup(x => x.ReadItemAsync<UserCurrentPlaylist>(It.IsAny<string>(), It.IsAny<PartitionKey>(), null, default))
                          .ThrowsAsync(new CosmosException("Not Found", System.Net.HttpStatusCode.NotFound, 404, "", 0));

            // Act
            var result = await _userPlaylistRepository.GetUserPlaylistAsync("user123");

            // Assert
            Assert.IsNull(result);
        }
    }
}
