//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Threading.Tasks;
//using Microsoft.Azure.Cosmos;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using MoviePlaylist.API.Models;
//using MoviePlaylist.API.Repositories;

//namespace MoviePlaylist.Tests
//{
//    [TestClass]
//    public class TrackRepositoryTests
//    {
//        private Mock<CosmosClient> _mockCosmosClient;
//        private Mock<Container> _mockContainer;
//        private TrackRepository _trackRepository;

//        [TestInitialize]
//        public void Setup()
//        {
//            // Mock the Cosmos client and container
//            _mockCosmosClient = new Mock<CosmosClient>();
//            _mockContainer = new Mock<Container>();
//            _mockCosmosClient.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
//                             .Returns(_mockContainer.Object);

//            // Initialize the repository with mocked container
//            _trackRepository = new TrackRepository(_mockCosmosClient.Object, "testDatabase", "testContainer");
//        }

//        [TestMethod]
//        public async Task AddTrackAsync_ShouldAddTrack()
//        {
//            // Arrange
//            var track = new Track { Id = "1", Title = "Track 1" };
//            _mockContainer.Setup(c => c.CreateItemAsync(It.IsAny<Track>(), null, default))
//                          .ReturnsAsync(new ItemResponse<Track>(track));

//            // Act
//            var result = await _trackRepository.AddTrackAsync(track);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(track.Id, result.Id);
//            Assert.AreEqual(track.Title, result.Title);
//        }

//        [TestMethod]
//        public async Task GetTrackByIdAsync_ShouldReturnTrack_WhenTrackExists()
//        {
//            // Arrange
//            var track = new Track { Id = "1", Title = "Track 1" };
//            _mockContainer.Setup(c => c.ReadItemAsync<Track>(track.Id, new PartitionKey(track.Id), null, default))
//                          .ReturnsAsync(new ItemResponse<Track>(track));

//            // Act
//            var result = await _trackRepository.GetTrackByIdAsync(track.Id);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(track.Id, result.Id);
//            Assert.AreEqual(track.Title, result.Title);
//        }

//        [TestMethod]
//        public async Task GetTrackByIdAsync_ShouldReturnNull_WhenTrackDoesNotExist()
//        {
//            // Arrange
//            var trackId = "non-existing-id";
//            _mockContainer.Setup(c => c.ReadItemAsync<Track>(trackId, new PartitionKey(trackId), null, default))
//                          .ThrowsAsync(new CosmosException("Not Found", System.Net.HttpStatusCode.NotFound, 0, "", 0));

//            // Act
//            var result = await _trackRepository.GetTrackByIdAsync(trackId);

//            // Assert
//            Assert.IsNull(result);
//        }

//        [TestMethod]
//        public async Task UpdateTrackAsync_ShouldUpdateTrack()
//        {
//            // Arrange
//            var track = new Track { Id = "1", Title = "Track 1" };
//            _mockContainer.Setup(c => c.ReplaceItemAsync(track, track.Id, null, default))
//                          .ReturnsAsync(new ItemResponse<Track>(track));

//            // Act
//            var result = await _trackRepository.UpdateTrackAsync(track.Id, track);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(track.Id, result.Id);
//        }

//        [TestMethod]
//        public async Task DeleteTrackAsync_ShouldReturnTrue_WhenTrackExists()
//        {
//            // Arrange
//            var trackId = "1";
//            _mockContainer.Setup(c => c.DeleteItemAsync<Track>(trackId, new PartitionKey(trackId), null, default))
//                          .ReturnsAsync(new ItemResponse<Track>(new Track { Id = trackId }));

//            // Act
//            var result = await _trackRepository.DeleteTrackAsync(trackId);

//            // Assert
//            Assert.IsTrue(result);
//        }

//        [TestMethod]
//        public async Task DeleteTrackAsync_ShouldReturnFalse_WhenTrackDoesNotExist()
//        {
//            // Arrange
//            var trackId = "non-existing-id";
//            _mockContainer.Setup(c => c.DeleteItemAsync<Track>(trackId, new PartitionKey(trackId), null, default))
//                          .ThrowsAsync(new CosmosException("Not Found", System.Net.HttpStatusCode.NotFound, 0, "", 0));

//            // Act
//            var result = await _trackRepository.DeleteTrackAsync(trackId);

//            // Assert
//            Assert.IsFalse(result);
//        }

//        [TestMethod]
//        public async Task GetAllTracksAsync_ShouldReturnAllTracks()
//        {
//            // Arrange
//            var tracks = new List<Track>
//            {
//                new Track { Id = "1", Title = "Track 1" },
//                new Track { Id = "2", Title = "Track 2" }
//            };

//            var iteratorMock = new Mock<FeedIterator<Track>>();
//            iteratorMock.SetupSequence(i => i.HasMoreResults)
//                        .Returns(true)
//                        .Returns(false);
//            iteratorMock.Setup(i => i.ReadNextAsync(default))
//                        .ReturnsAsync(ResponseMessage.CreateSuccessResponse(tracks));

//            _mockContainer.Setup(c => c.GetItemQueryIterator<Track>(null, null, null))
//                          .Returns(iteratorMock.Object);

//            // Act
//            var result = await _trackRepository.GetAllTracksAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(2, result.Count);
//        }
//    }
//}
