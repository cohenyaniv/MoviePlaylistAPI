using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using MoviePlaylist.Models;
using MoviePlaylist.Repositories;
using MoviePlaylist.Services;
using System;
using System.Threading.Tasks;

namespace MoviePlaylist.Tests.Services
{
    [TestClass]
    public class PlaylistServiceUnitTests
    {
        private Mock<IPlaylistRepository> _playlistRepositoryMock;
        private Mock<IUserPlaylistRepository> _userPlaylistRepositoryMock;
        private Mock<IUserHistoryRepository> _userHistoryRepositoryMock;
        private Mock<IQueueService> _queueServiceMock;
        private Mock<UserCounterService> _userCounterServiceMock;
        private PlaylistService _playlistService;

        [TestInitialize]
        public void Setup()
        {
            // Mock dependencies
            _playlistRepositoryMock = new Mock<IPlaylistRepository>();
            _userPlaylistRepositoryMock = new Mock<IUserPlaylistRepository>();
            _queueServiceMock = new Mock<IQueueService>();
            _userCounterServiceMock = new Mock<UserCounterService>();
            _userHistoryRepositoryMock = new Mock<IUserHistoryRepository>();

            // Instantiate PlaylistService with mocked dependencies
            _playlistService = new PlaylistService(
                _playlistRepositoryMock.Object,
                _userPlaylistRepositoryMock.Object,
                _queueServiceMock.Object,
                _userCounterServiceMock.Object,
                _userHistoryRepositoryMock.Object
            );
        }

        [TestMethod]
        public async Task GetPlaylistByIdAsync_ShouldReturnPlaylist_WhenIdIsValid()
        {
            // Arrange
            string validPlaylistId = "123";
            var playlist = new Playlist { PlaylistId = validPlaylistId };
            _playlistRepositoryMock.Setup(x => x.GetPlaylistByIdAsync(validPlaylistId))
                .ReturnsAsync(playlist);

            // Act
            var result = await _playlistService.GetPlaylistByIdAsync(validPlaylistId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(validPlaylistId, result.PlaylistId);
            _playlistRepositoryMock.Verify(x => x.GetPlaylistByIdAsync(validPlaylistId), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetPlaylistByIdAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Act
            await _playlistService.GetPlaylistByIdAsync(string.Empty);
        }

        [TestMethod]
        public async Task StartPlaylistAsync_ShouldStartCounterAndQueuePlaylist_WhenPlaylistIsAttached()
        {
            // Arrange
            string userId = "user123";
            var userPlaylist = new UserCurrentPlaylist { UserId = userId, Status = PlaylistStatus.Attached };
            _userPlaylistRepositoryMock.Setup(x => x.GetUserPlaylistAsync(userId))
                .ReturnsAsync(userPlaylist);

            // Act
            var result = await _playlistService.StartPlaylistAsync("playlist123", userId);

            // Assert
            Assert.IsTrue(result);
            _queueServiceMock.Verify(x => x.QueueUserPlaylist(userPlaylist), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Playlist not attached, please attach first.")]
        public async Task StartPlaylistAsync_ShouldThrowException_WhenPlaylistNotAttached()
        {
            // Arrange
            string userId = "user123";
            _userPlaylistRepositoryMock.Setup(x => x.GetUserPlaylistAsync(userId))
                .ReturnsAsync((UserCurrentPlaylist)null);

            // Act
            await _playlistService.StartPlaylistAsync("playlist123", userId);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Playlist not found.")]
        public async Task StopPlaylistAsync_ShouldThrowException_WhenPlaylistNotFound()
        {
            // Arrange
            string playlistId = "invalidId";
            _playlistRepositoryMock.Setup(x => x.GetPlaylistByIdAsync(playlistId))
                .ReturnsAsync((Playlist)null);

            // Act
            await _playlistService.StopPlaylistAsync(playlistId, "user123");
        }

        [TestMethod]
        public async Task AttachPlaylistToUserAsync_ShouldAttachPlaylist_WhenPlaylistIsNotAttached()
        {
            // Arrange
            string playlistId = "playlist123";
            string userId = "user123";
            var playlist = new Playlist { PlaylistId = playlistId };
            _playlistRepositoryMock.Setup(x => x.GetPlaylistByIdAsync(playlistId))
                .ReturnsAsync(playlist);
            _userPlaylistRepositoryMock.Setup(x => x.GetUserPlaylistAsync(userId))
                .ReturnsAsync((UserCurrentPlaylist)null);

            // Act
            var result = await _playlistService.AttachPlaylistToUserAsync(playlistId, userId);

            // Assert
            Assert.IsTrue(result);
            _queueServiceMock.Verify(x => x.QueueUserPlaylist(It.IsAny<UserCurrentPlaylist>()), Times.Once);
        }
    }
}
