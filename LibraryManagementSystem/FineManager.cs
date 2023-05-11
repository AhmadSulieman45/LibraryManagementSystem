using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{

    public enum PaymentStatus
    {
        Sucess,
        Invalid,
        Empty
    }
    class FineManager
    {
        public Dictionary<int, int> DueFees
        {
            get;
            private set;
        }

        private Dictionary<int, DateTime> _lastSent; // reqId, sent notification Date

        private int GetElapsedInDays(DateTime first, DateTime second)
        {
            TimeSpan elapsed = second - first;
            return (int)elapsed.TotalDays;
        }

        private int CalculateDueFees(Request request)
        {
            TimeSpan elapsed = request.getRemainingTime();
            int days = (int)elapsed.TotalDays;
            return (days - Constants.BorrowTime + 1) * Constants.DuePenalty;
        }

        public void TrackFees(List<Request> requests) 
        {
            DueFees.Clear();
            foreach (Request req in requests)
            {
                if (req.CurrentStatus != Status.Due) continue;
                int fines = CalculateDueFees(req);

                if (!DueFees.ContainsKey(req.UserID))
                {
                    DueFees[req.UserID] = 0;
                }

                DueFees[req.UserID] += fines;
            }
        }

        public PaymentStatus PayFine(int userId, int amount)
        {
            if (!DueFees.ContainsKey(userId)) return PaymentStatus.Empty;

            if (DueFees[userId] > amount) return PaymentStatus.Invalid;

            return PaymentStatus.Sucess;
        }

        public Dictionary<int, int> GenerateReport(List<Request> requests)
        {
            TrackFees(requests);
            return DueFees;
        }
    }
}
