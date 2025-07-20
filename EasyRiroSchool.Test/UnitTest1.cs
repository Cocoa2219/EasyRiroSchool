using EasyRiroSchool.API;
using EasyRiroSchool.API.Exceptions;
using EasyRiroSchool.Models;
using EasyRiroSchool.Models.Deserialization.Items;

namespace EasyRiroSchool.Test;

public class UnitTest1
{
    private RiroSchool _riroschool;

    [SetUp]
    public void Setup()
    {
        _riroschool = new RiroSchool();
    }

    private async Task LoginAsync(string email, string password)
    {
        await _riroschool.LoginAsync(email, password);
        Assert.That(_riroschool.Token, Is.Not.Empty, "Token should not be empty after login.");
    }

    [Test]
    [TestCase("cocoa.2324a@gmail.com", "cocoa2219!")]
    public async Task Login_WithValidCredentials_ShouldSucceed(string email, string password)
    {
        await LoginAsync(email, password);
    }

    [Test]
    [TestCase("invalid_user", "invalid_password")]
    public async Task Login_WithInvalidCredentials_ShouldFail(string email, string password)
    {
        var ex = Assert.ThrowsAsync<RiroLoginException>(() => _riroschool.LoginAsync(email, password));
        Assert.That(ex?.Message, Is.Not.Empty, "Exception message should not be empty.");
    }

    private static IEnumerable<TestCaseData> GetTableTestCases()
    {
        yield return new TestCaseData(
            new DbInfo(DbId.CurricularActivity),
            typeof(PortfolioItem)
        ).SetName("GetPortfolio_ShouldSucceed");

        yield return new TestCaseData(
            new DbInfo(DbId.Announcement),
            typeof(BoardItem)
        ).SetName("GetBoard_ShouldSucceed");

        yield return new TestCaseData(
            new DbInfo(DbId.MealApplication),
            typeof(MealApplicationItem)
        ).SetName("GetMealApplication_ShouldSucceed");
    }

    [Test]
    [TestCaseSource(nameof(GetTableTestCases))]
    public async Task GetTable_GenericTest(DbInfo dbInfo, Type itemType)
    {
        await LoginAsync("cocoa.2324a@gmail.com", "cocoa2219!");

        var method = typeof(RiroSchool)
            .GetMethod(nameof(RiroSchool.GetTableAsync))!
            .MakeGenericMethod(itemType);

        var task = (Task)method.Invoke(_riroschool, [dbInfo])!;
        await task.ConfigureAwait(false);

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = (IEnumerable<object>)resultProperty.GetValue(task)!;

        Assert.That(result, Is.Not.Null.And.Not.Empty, $"Table data for {itemType.Name} should not be null or empty.");

        foreach (var item in result)
        {
            Console.WriteLine(item);
        }
    }

    [Test]
    public async Task GetCalendar_ShouldSucceed()
    {
        await LoginAsync("cocoa.2324a@gmail.com", "cocoa2219!");

        var calendar = await _riroschool.GetCalendarAsync(2025, 7);
        Assert.That(calendar, Is.Not.Null, "Calendar should not be null.");
        Assert.That(calendar, Is.Not.Empty, "Calendar should contain items.");

        foreach (var item in calendar)
        {
            Console.WriteLine($"Date: {item.Date}, Events: {string.Join(", ", item.Events)}, IsHoliday: {item.IsHoliday}");
        }
    }
}
