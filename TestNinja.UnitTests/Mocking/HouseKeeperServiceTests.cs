using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.Mocking
{
    [TestFixture]
    public class HouseKeeperServiceTests
    {
        private HousekeeperService _service;
        private Mock<IStatementGenerator> _statementGenerate;
        private Mock<IEmailSender> _emailSender;
        private Mock<IXtraMessageBox> _messageBox;
        private DateTime _statementDate = new DateTime(2017,1,1);
        private Housekeeper _houseKeeper;
        private string _statementFilename;

        [SetUp]
        public void Setup()
        {
            _houseKeeper = new Housekeeper{Email = "a", FullName = "b", StatementEmailBody = "c", Oid = 1};
            
            var unitOfWork = new Mock<IUnitOfWork>();
            
            unitOfWork.Setup(u => u.Query<Housekeeper>())
                .Returns(new List<Housekeeper>
                {
                    _houseKeeper
                }.AsQueryable());

            _statementFilename = "filename";
            _statementGenerate = new Mock<IStatementGenerator>();
            _statementGenerate.
                Setup(sg=> sg.SaveStatement(_houseKeeper.Oid,_houseKeeper.FullName,_statementDate))
                .Returns(()=>_statementFilename);
            
            _emailSender = new Mock<IEmailSender>();
           _messageBox = new Mock<IXtraMessageBox>();
           
           _service = new HousekeeperService(unitOfWork.Object, _statementGenerate.Object, _emailSender.Object, _messageBox.Object);

        }

        [Test]
        public void SendStatementEmails_WhenCalled_GenerateStatements()
        {
           
            _service.SendStatementEmails(_statementDate);
            
            _statementGenerate.
                Verify(sg=> 
                    sg.SaveStatement(_houseKeeper.Oid,_houseKeeper.FullName,_statementDate));
        }
        
        [Test]
        public void SendStatementEmails_HouseKeeperEmailIsNull_ShouldNotGenerateStatements()
        {
            _houseKeeper.Email = null;
            
            _service.SendStatementEmails(_statementDate);
            
            _statementGenerate.
                Verify(sg=> 
                    sg.SaveStatement(_houseKeeper.Oid,_houseKeeper.FullName,_statementDate),Times.Never);
        }
        
        [Test]
        public void SendStatementEmails_HouseKeeperEmailIsWhiteSpace_ShouldNotGenerateStatements()
        {
            _houseKeeper.Email = " ";

            _service.SendStatementEmails(_statementDate);
            
            _statementGenerate.
                Verify(sg=> 
                    sg.SaveStatement(_houseKeeper.Oid,_houseKeeper.FullName,_statementDate),Times.Never);
        }
        
        [Test]
        public void SendStatementEmails_HouseKeeperEmailIsEmpty_ShouldNotGenerateStatements()
        {
            _houseKeeper.Email = "";

            _service.SendStatementEmails(_statementDate);
            
            _statementGenerate.
                Verify(sg=> 
                    sg.SaveStatement(_houseKeeper.Oid,_houseKeeper.FullName,_statementDate),Times.Never);
        }
        
        [Test]
        public void SendStatementEmails_WhenCalled_EmailTheStatement()
        {
            _service.SendStatementEmails(_statementDate);
            
            VerifyEmailSend();
        }
        
        [Test]
        public void SendStatementEmails_StatementFileNameIsNull_ShouldNotEmailTheStatement()
        {
            _statementFilename = null;
            _service.SendStatementEmails(_statementDate);
            
            VerifyEmailNotSend();
        }
        

        [Test]
        public void SendStatementEmails_StatementFileNameIsEmptyString_ShouldNotEmailTheStatement()
        {
            _statementFilename = "";
            
            _service.SendStatementEmails(_statementDate);
            
          VerifyEmailNotSend();
        }
        
        [Test]
        public void SendStatementEmails_StatementFileNameIsWhiteSpace_ShouldNotEmailTheStatement()
        {
            _statementFilename = " ";
            
            _service.SendStatementEmails(_statementDate);
            
            VerifyEmailNotSend();
        }
        
        private void VerifyEmailNotSend()
        {
            _emailSender
                .Verify(es => es.EmailFile(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()), Times.Never);
        }
        
        private void VerifyEmailSend()
        {
            _emailSender
                .Verify(es => es.EmailFile(
                    _houseKeeper.Email,
                    _houseKeeper.StatementEmailBody,
                    _statementFilename, It.IsAny<string>()));
        }
    }
}