﻿namespace DotNet7.ExpenseTrackerApi.BlazorWasm.Models;

public class KeyValueModel
{
    public string Key { get; set; }
    public string Value { get; set; }

    public KeyValueModel(string key, string value)
    {
        Key = key;
        Value = value;
    }
}