using Medicare.Domain.Entities;

namespace Medicare.Domain.Services
{
    public static class OrderStatusTransitionValidator
    {
        private static readonly IReadOnlyDictionary<OrderStatus, HashSet<OrderStatus>> ValidTransitions =
            new Dictionary<OrderStatus, HashSet<OrderStatus>>
            {
                { OrderStatus.New, new HashSet<OrderStatus> { OrderStatus.Contacted, OrderStatus.Cancelled } },
                { OrderStatus.Contacted, new HashSet<OrderStatus> { OrderStatus.Confirmed, OrderStatus.Cancelled } },
                { OrderStatus.Confirmed, new HashSet<OrderStatus> { OrderStatus.Completed, OrderStatus.Cancelled } },
                { OrderStatus.Completed, new HashSet<OrderStatus>() },
                { OrderStatus.Cancelled, new HashSet<OrderStatus>() },
            };

        public static bool IsValidTransition(OrderStatus from, OrderStatus to)
        {
            return ValidTransitions.TryGetValue(from, out var transitions) && transitions.Contains(to);
        }
    }
}