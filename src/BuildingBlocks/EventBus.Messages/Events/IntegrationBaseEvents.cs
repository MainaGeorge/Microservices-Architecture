using System;

namespace EventBus.Messages.Events
{
    public class IntegrationBaseEvents
    {
        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }

        public IntegrationBaseEvents(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        public IntegrationBaseEvents()
        {
            Id = new Guid();
            CreationDate = DateTime.UtcNow;
        }
    }
}
