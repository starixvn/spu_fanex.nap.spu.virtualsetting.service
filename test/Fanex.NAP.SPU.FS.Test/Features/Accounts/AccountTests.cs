namespace Fanex.NAP.SPU.DCS.Tests.Features.Accounts
{
    public class AccountTests
    {
        //private readonly IAccountDataAccess accountDataAccess;
        //private readonly IAccountServices accountServices;

        //public AccountTests()
        //{
        //    accountDataAccess = Substitute.For<IAccountDataAccess>();

        //    accountServices = new BigStateNearGoalServices(accountDataAccess);
        //}



        //[Fact]
        //public void GetRawAccount_WhenSuccess_ReturnAccountList()
        //{
        //    var accountEntity = new List<AccountEntity> {
        //        new AccountEntity
        //        {
        //           AccountId = 1,
        //           BatchId = 1,
        //           LastLoginTime = DateTime.Now,
        //           LoginName = "abc",
        //           SubscriberId = 1
        //        },
        //         new AccountEntity
        //        {
        //           AccountId = 2,
        //           BatchId = 1,
        //           LastLoginTime = DateTime.Now,
        //           LoginName = "abczzz",
        //           SubscriberId = 1
        //        },
        //          new AccountEntity
        //        {
        //           AccountId = 3,
        //           BatchId = 1,
        //           LastLoginTime = DateTime.Now,
        //           LoginName = "abcxxx",
        //           SubscriberId = 1
        //        },
        //        new AccountEntity
        //        {
        //           AccountId = 4,
        //           BatchId = 2,
        //           LastLoginTime = DateTime.Now,
        //           LoginName = "xxx",
        //           SubscriberId = 1
        //        },
        //         new AccountEntity
        //        {
        //           AccountId = 5,
        //           BatchId = 2,
        //           LastLoginTime = DateTime.Now,
        //           LoginName = "xxxabc",
        //           SubscriberId = 1
        //        },
        //          new AccountEntity
        //        {
        //           AccountId = 6,
        //           BatchId = 2,
        //           LastLoginTime = DateTime.Now,
        //           LoginName = "xxxzzz",
        //           SubscriberId = 1
        //        },
        //    };

        //    accountDataAccess.GetRawAccount(100, 2).Returns(accountEntity);

        //    var result = accountServices.GetRawAccount(100, 2);

        //    result.Should().BeEquivalentTo(accountEntity);
        //}

        //[Theory]
        //[InlineData(-1, 1)]
        //[InlineData(1, -1)]
        //[InlineData(-1, -1)]
        //[InlineData(0, 0)]
        //[InlineData(0, -1)]
        //[InlineData(-1, 0)]
        //public void GetStagingTransIdForDevice_WhenFailed_ReturnTransIdList(int size, int batchSize)
        //{
        //    var expectedResult = new List<StagingTransIdEntity>();

        //    deviceDataAccess.GetStagingTransIdForDevice(size, batchSize).Returns(expectedResult);

        //    var result = deviceServices.GetStagingTransIdForDevice(size, batchSize);

        //    result.Should().BeEmpty();
        //    result.Should().BeEquivalentTo(expectedResult);
        //    result.Count().Should().Be(0);
        //}

        //[Fact]
        //public void UpdateDevice_WhenSucess_ReturnDataResult()
        //{
        //    var stagingTransIdEntity = new List<StagingTransIdEntity> {
        //        new StagingTransIdEntity
        //        {
        //            BatchId = 1,
        //            TransId = 100
        //        },
        //        new StagingTransIdEntity
        //        {
        //            BatchId = 1,
        //            TransId = 200
        //        },
        //        new StagingTransIdEntity
        //        {
        //            BatchId = 1,
        //            TransId = 300
        //        }
        //    };
        //    var dataResult = new DataResult {
        //        ErrorCode = 0,
        //        ErrorMessage = string.Empty
        //    };
        //    var jsonResult = JsonConvert.SerializeObject(stagingTransIdEntity);

        //    deviceDataAccess.UpdateDevice(jsonResult).Returns(dataResult);

        //    var result = deviceServices.UpdateDevices(stagingTransIdEntity);

        //    result.Should().Be(dataResult);
        //    result.ErrorCode.Should().Be(0);
        //}

        //[Fact]
        //public void UpdateDevice_WhenFailed_ReturnDataResult()
        //{
        //    var dataResult = new DataResult
        //    {
        //        ErrorCode = -1,
        //        ErrorMessage = "Parameter is empty."
        //    };

        //    deviceDataAccess.UpdateDevice(null).Returns(dataResult);
        //    var result = deviceServices.UpdateDevices(null);

        //    result.ErrorCode.Should().Be(-1);
        //}
    }
}
