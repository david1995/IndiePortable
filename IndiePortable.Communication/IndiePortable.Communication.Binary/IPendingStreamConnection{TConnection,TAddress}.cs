namespace IndiePortable.Communication.Binary
{
    public interface IPendingStreamConnection<TConnection, TAddress>
        where TConnection : IStreamConnection
    {
        TAddress SourceAddress { get; }

        StreamConnectionClient<TConnection, TAddress> Accept();

        void Deny();
    }
}
