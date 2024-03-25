using OtpNet;
namespace Qydha.Domain.Services.Implementation;

public class OtpManager
{
    private readonly Totp _tOtp;
    private readonly OTPSettings _otpSettings;
    public OtpManager(IOptions<OTPSettings> OTPSettings)
    {
        _otpSettings = OTPSettings.Value;
        _tOtp = new Totp(Base32Encoding.ToBytes(_otpSettings.Secret));
    }

    public string GenerateOTP() => _tOtp.ComputeTotp();

    public bool IsOtpValid(DateTimeOffset createOn) => (DateTimeOffset.UtcNow - createOn).TotalSeconds <= _otpSettings.TimeInSec;


}
