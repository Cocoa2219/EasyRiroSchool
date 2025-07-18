namespace EasyRiroSchool.API.Models;

/// <summary>
/// Enumeration representing various database IDs used in the EasyRiroSchool API.
/// </summary>
public enum DbId
{
    /// <summary>
    /// Represents no specific database ID.
    /// </summary>
    None = 0,

    /// <summary>
    /// 개인별 공지 / 개인별 공지
    /// </summary>
    IndividualAnnouncement = 1841,

    /// <summary>
    /// 알림 신청 / 가정통신문(전학년)
    /// </summary>
    Announcement = 1901,

    /// <summary>
    /// 알림 신청 / 1학년
    /// </summary>
    FirstGradeAnnouncement = 1902,

    /// <summary>
    /// 알림 신청 / 2학년
    /// </summary>
    SecondGradeAnnouncement = 1903,

    /// <summary>
    /// 알림 신청 / 3학년
    /// </summary>
    ThirdGradeAnnouncement = 1904,

    /// <summary>
    /// 알림 신청 / 설문조사
    /// </summary>
    Votes = 1906,

    /// <summary>
    /// 알림 신청 / 급식 신청
    /// </summary>
    MealApplication = 1951,

    /// <summary>
    /// 알림 신청 / 통학버스 신청
    /// </summary>
    BusApplication = 1907,

    /// <summary>
    /// 알림 신청 / 특별실.상담 예약
    /// </summary>
    CounselingApplication = 1961,

    /// <summary>
    /// 알림 신청 / 학사 일정
    /// </summary>
    SchoolSchedule = 2301,

    /// <summary>
    /// 알림 신청 / 오늘의식단
    /// </summary>
    MealCalendar = 2303,

    /// <summary>
    /// 알림 신청 / 결석신고서
    /// </summary>
    AbsenceReport = 2302,

    /// <summary>
    /// 활동보고서 / 포트폴리오
    /// </summary>
    Portfolio = 1502,

    /// <summary>
    /// 활동보고서 / 교과 활동
    /// </summary>
    CurricularActivity = 1551,

    /// <summary>
    /// 활동보고서 / 경시대회
    /// </summary>
    Contest = 1552,
}