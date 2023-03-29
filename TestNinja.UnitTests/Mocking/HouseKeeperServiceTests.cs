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

        [Test]
        public void SendStatementEmails_WhenCalled_GenerateStatements()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(u => u.Query<Housekeeper>())
                .Returns(new List<Housekeeper>
                {
                    new Housekeeper{Email = "a", FullName = "b", StatementEmailBody = "c", Oid = 1}
                }.AsQueryable());

            var statementGenerate = new Mock<IStatementGenerator>();
            var emailSender = new Mock<IEmailSender>();
            var messageBox = new Mock<IXtraMessageBox>();
            
            var service = new HousekeeperService(unitOfWork.Object, statementGenerate.Object, emailSender.Object, messageBox.Object);
            
            service.SendStatementEmails(new DateTime(2017,1,1));
            
            statementGenerate.
                Verify(sg=> 
                    sg.SaveStatement(1,"b",new DateTime(2017,1,1)));
        }
    }
}