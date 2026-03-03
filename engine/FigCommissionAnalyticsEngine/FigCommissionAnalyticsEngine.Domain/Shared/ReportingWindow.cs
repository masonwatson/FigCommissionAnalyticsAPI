using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigCommissionAnalyticsEngine.Domain.Shared
{
    public sealed class ReportingWindow
    {
        public DateOnly StartDate { get; }
        public DateOnly EndDate { get; }
        
        private ReportingWindow(DateOnly startDate, DateOnly endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public static ReportingWindow? Create(DateOnly? startDate, DateOnly? endDate)
        {
            if (startDate.HasValue != endDate.HasValue)
            {
                throw new ArgumentException("Both StartDate and EndDate must be provided together, or neither should be provided.");
            }

            if (endDate < startDate)
            {
                throw new ArgumentException("End date must be greater than or equal to start date.");
            }

            if (!startDate.HasValue && !endDate.HasValue)
            {
                return null;
            }

            return new ReportingWindow(startDate!.Value, endDate!.Value);
        }
    }
}
