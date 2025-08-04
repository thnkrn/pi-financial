namespace Pi.StructureNotes.Infrastructure.Services;

public class S3NoteReaderConfig
{
    public string Bucket { get; init; }
    public string Prefix { get; init; }
    public string RoleArn { get; init; }
    public string Key { get; init; }
    public string Secret { get; init; }
}
