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

    public class FineManager
    {
        public Dictionary<int, int> DueFees
        {
            get;
            set;
        }

        public FineManager()
        {
            DueFees = new Dictionary<int, int>();
        }

        private int CalculateDueFees(Request request)
        {
            TimeSpan elapsed = request.GetTimeBorrowed();
            int days = (int)Math.Ceiling(elapsed.TotalDays);
            return (days - Constants.BorrowTime) * Constants.DuePenalty;
        }

        public void TrackFees(List<Request> requests) 
        {
            DueFees.Clear();
            foreach (Request req in requests)
            {
                if (req.CurrentStatus != RequestType.Due && req.CurrentStatus != RequestType.ReturnDue) continue;
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
