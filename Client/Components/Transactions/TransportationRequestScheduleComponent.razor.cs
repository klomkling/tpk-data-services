using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Blazor;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components.Transactions
{
    public class TransportationRequestScheduleComponentBase : TgMinimalComponentBase
    {
        protected IEnumerable<TransportationRequestSchedule> Appointments { get; set; } 
        protected DxSchedulerTimeSpanRange WorkTime = new DxSchedulerTimeSpanRange(TimeSpan.FromHours(8), TimeSpan.FromHours(17));
        protected DxSchedulerDataStorage DataStorage = new DxSchedulerDataStorage()
        {
            AppointmentsSource = null,
            AppointmentMappings = new DxSchedulerAppointmentMappings
            {
                Type = "AppointmentType",
                Start = "StartDate",
                End = "EndDate",
                Subject = "Caption",
                AllDay = "AllDay",
                Location = "Location",
                Description = "Description",
                LabelId = "Label",
                StatusId = "Status",
                RecurrenceInfo = "Recurrence"
            }
        };
        
        protected override async Task OnInitializedAsync()
        {
            var result = await LoadAppointments();
            DataStorage.AppointmentsSource = result;
        }

        protected async Task<List<TransportationRequestSchedule>> LoadAppointments()
        {
            var result = await ApiService.GetAllAsync<TransportationRequestSchedule>("api/transportation-requests/schedules");
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
            return result?.ToList() ?? new List<TransportationRequestSchedule>();
        }
    }
}