namespace EasyRiroSchool.Models.Deserialization;

[Flags]
public enum Grade
{
    None = 0,
    First = 1 << 0,
    Second = 1 << 1,
    Third = 1 << 2,
    All = First | Second | Third
}