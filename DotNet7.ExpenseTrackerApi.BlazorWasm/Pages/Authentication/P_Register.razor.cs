using DotNet7.ExpenseTrackerApi.BlazorWasm.Models.User;
using System.Text.RegularExpressions;

namespace DotNet7.ExpenseTrackerApi.BlazorWasm.Pages.Authentication
{
    public partial class P_Register
    {
        RegisterRequestModel requestModel = new();
        private bool isBtnDisabled = true;

        private void Validate()
        {
            isBtnDisabled = IsNullOrEmpty(requestModel.UserName) || IsNullOrEmpty(requestModel.Email)
                || !IsValidEmail(requestModel.Email)
                || IsNullOrEmpty(requestModel.Password) || requestModel.DOB == default
                || IsNullOrEmpty(requestModel.Gender);
        }

        private bool IsNullOrEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        private bool IsValidEmail(string str)
        {
            return Regex.IsMatch(str, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}