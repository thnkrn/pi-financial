using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pi.Common.Cryptography;
using Pi.User.Application.Options;

namespace Pi.User.API.Controllers;

[ApiController]
[Route("internal/debug")]
public class DebugController : ControllerBase
{
    private readonly IEncryption encryption;
    private readonly IOptions<DbConfig> dbConfig;

    public DebugController(IEncryption encryption, IOptions<DbConfig> dbConfig)
    {
        this.encryption = encryption;
        this.dbConfig = dbConfig;
    }

    [HttpGet("hash")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult Hash([FromQuery] string input)
    {
        var result = encryption.Hashed(input, dbConfig.Value.Salt);
        return Ok(result);
    }
}
