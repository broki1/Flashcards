﻿using Azure.Core.Pipeline;

namespace Flashcards.Models;

internal class YearReport
{

    public string StackName { get; set; }
    public int January { get; set; }
    public int February { get; set; }
    public int March { get; set;  }
    public int April { get; set; }
    public int May {  get; set; }
    public int June { get; set; }
    public int July { get; set;}
    public int August { get; set; }
    public int September { get; set; }
    public int October { get; set; }
    public int November { get; set; }
    public int December { get; set; }

    public YearReport()
    {
        January = 0;
        February = 0;
        March = 0;
        April = 0;
        May = 0;
        June = 0;
        July = 0;
        August = 0;
        September = 0;
        October = 0;
        November = 0;
        December = 0;
    }

}
