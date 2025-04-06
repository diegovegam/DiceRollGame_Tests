using Game;
using Moq;
using NUnit.Framework;
using System.Security.Cryptography.X509Certificates;
using UserCommunication;

namespace DiceRollGame_Tests
{
    [TestFixture]
    public class GuessingGameTests
    {
        private Mock<IUserCommunication> _userCommunicationMock;
        private Mock<IDice> _diceMock;
        private GuessingGame _guessingGame;

        [SetUp]
        public void Setup()
        {
            _userCommunicationMock = new Mock<IUserCommunication>();
            _diceMock = new Mock<IDice>();
            _guessingGame = new GuessingGame(_diceMock.Object, _userCommunicationMock.Object);
        }

        [Test]
        public void Play_ShallReturnVictory_GuessingItTheFirstTime()
        {
     
            _userCommunicationMock
                .Setup(uCmock => uCmock.ReadInteger(It.IsAny<string>()))
                .Returns(1);
            _diceMock
                .Setup(dicemock => dicemock.Roll())
                .Returns(1);

            var actualResult = _guessingGame.Play();

            Assert.AreEqual(GameResult.Victory, actualResult);
        }

        [Test]
        public void Play_ShallReturnVictory_GuessingItTheSecondTime()
        {
            _diceMock
                .Setup(dicemock => dicemock.Roll())
                .Returns(3);
           
            _userCommunicationMock
                .SetupSequence(uCmock => uCmock.ReadInteger(It.IsAny<string>()))
                .Returns(1)
                .Returns(3);

            var actualResult = _guessingGame.Play();

            Assert.AreEqual(GameResult.Victory, actualResult);
        }

        public GameResult PlayWithThreeTries ()
        {
            _diceMock
                .Setup(dicemock => dicemock.Roll())
                .Returns(3);

            _userCommunicationMock
                .SetupSequence(uCmock => uCmock.ReadInteger(It.IsAny<string>()))
                .Returns(1)
                .Returns(2)
                .Returns(3);

            return _guessingGame.Play();
        }

        [Test]
        public void Play_ShallReturnVictory_GuessingItTheThirdTime()
        {
            var actualResult = PlayWithThreeTries();

            _userCommunicationMock.Verify(
                uCmock => uCmock.ShowMessage("Wrong number."), Times.Exactly(2));

            Assert.AreEqual(GameResult.Victory, actualResult);
        }

        [Test]
        public void Play_ShallShowMessage3Times_WhenNotGuessingIt()
        {
            _userCommunicationMock
                .Setup(uCmock => uCmock.ReadInteger(It.IsAny<string>()))
                .Returns(1);
            _diceMock
                .Setup(dicemock => dicemock.Roll())
                .Returns(3);

            var actualResult = _guessingGame.Play();

            _userCommunicationMock.Verify(
                uCmock => uCmock.ShowMessage("Wrong number."), Times.Exactly(3));
        }

        [Test]
        public void Play_ShallReadInteger3Times_WhenGuessingItTheThirdTime()
        {

             PlayWithThreeTries();

            _userCommunicationMock.Verify(
                uCmock => uCmock.ReadInteger("Enter a number:"), Times.Exactly(3));

        }

        [Test]
        public void Play_ShallReturnLoss_WhenNotGuessingIt()
        {
            _userCommunicationMock
                .Setup(uCmock => uCmock.ReadInteger(It.IsAny<string>()))
                .Returns(1);
            _diceMock
                .Setup(dicemock => dicemock.Roll())
                .Returns(3);

            var actualResult = _guessingGame.Play();

            Assert.AreEqual(GameResult.Loss, actualResult);
        }


        [TestCase(GameResult.Victory, "You win!")]
        [TestCase(GameResult.Loss, "You lose :(")]
        public void PrintResult_ShallPrintYouWinOrLose_WhenVictoryOrLossIsPassed(
            GameResult gameResult, string messageShown)
        {
            _guessingGame.PrintResult(gameResult);

            _userCommunicationMock.Verify(
                mock => mock.ShowMessage(messageShown));
        }

        [Test]
        public void Play_ShallShowWelcomeMessage()
        {
            _guessingGame.Play();
            _userCommunicationMock.Verify(
                mock => mock.ShowMessage($"Dice rolled. Guess what number it shows in 3 tries."),
                Times.Once);
        }
    }
}
