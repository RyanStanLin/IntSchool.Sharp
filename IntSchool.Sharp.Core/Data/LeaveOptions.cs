using System.ComponentModel;

namespace IntSchool.Sharp.Core;

public enum LeaveOptions : short
{
    SickLeave = 22,
    Hospital = 23,
    DoctorAppointment = 24,
    SickLeavePersonal = 25,
    Holidays = 26,
    Exam = 27,
    Others = 28
}

public enum LeaveTypes
{
    [Description(Constants.LeaveTypeIllness)]
    Illness,
    [Description(Constants.LeaveTypePersonal)]
    Personal
}