namespace Pi.OnePort.TCP.Helpers;

internal class DeserializationHelper
{
    public int Start { get; private set; }
    public int End { get; private set; }

    private readonly string _data;

    public DeserializationHelper(string data)
    {
        _data = data;
    }

    private void Increment(int amount)
    {
        Start = End;
        End += amount;
    }

    public string Next(int amount)
    {
        Increment(amount);
        return _data[Start..End];
    }

    public bool TryNew(int amount, out string output)
    {
        Increment(amount);
        output = _data[Start..End];

        return output.Trim() != "";
    }
}
