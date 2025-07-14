using System;

using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Modules
{

    TEACHERS,
    DASHBOARD,
    SETTINGS,
    STUDENTS,
    PARENTS,
    SUPPORT,
    SUGGESTION,
    CLASSROOMMANAGEMENT,
    VIRTUALCLASSROOM,
    ASSIGNMENT,
    VIRTUALMEETING,
    EXAMS,
    LESSONPLAN,
    ADMISSIONS,
    LIBRARY,
    CALENDAR,
    FEES,
    ACCOUNTMANAGEMENT,
    BROADCAST,
    MESSAGING,
    CONFIGURATION,
    AUDIT


}
