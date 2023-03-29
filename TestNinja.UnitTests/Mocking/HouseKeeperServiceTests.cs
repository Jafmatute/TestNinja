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
            
            _statementGenerate = new Mock<IStatementGenerator>();
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
    }
}