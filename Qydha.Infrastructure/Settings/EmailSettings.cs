namespace Qydha.Infrastructure.Settings;

public class EmailSettings
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string ConfirmUrl { get; set; } = string.Empty;
    public string ConfirmEmailTemplate { get; } = @"
        <!DOCTYPE html>
        <html lang='ar' dir='rtl'>

        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>تاكيد البريد الالكتروني</title>
            <style>
                @import url('https://fonts.googleapis.com/css2?family=Cairo:wght@400;800&display=swap');

                * {
                    box-sizing: border-box;
                    padding: 0;
                    margin: 0;
                }

                body {
                    font-family: 'Cairo', sans-serif;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }


                #main-container {
                    width: 100vw;
                    height: 100vh;
                    overflow: hidden;
                    display: flex;

                }

                #data-container {
                    width: 50%;
                    height: 100%;
                    padding: 3rem 5rem;
                    display: flex;
                    justify-content: start;
                    align-items: center;
                    flex-direction: column;
                }

                #svg-container {
                    width: 50%;

                }

                #login-img {
                    height: 100%;
                    width: 100%;
                    object-fit: cover;
                    object-position: center;
                }

                #qydha-logo-img {
                    width: 7rem;
                    align-self: self-start;
                }

                #header {
                    font-size: 2.1rem;
                    background: linear-gradient(166.5deg, #222751 46.85%, #9CAAF3 118.94%);
                    background-clip: text;
                    -webkit-background-clip: text;
                    -webkit-text-fill-color: transparent;
                    margin-bottom: 0;
                    padding-bottom: 0;
                }

                #disc {
                    font-size: 1.25rem;
                    color: gray;
                }

                #code {
                    color: #222751;
                    font-size: 1.75rem;
                    font-weight: 500;
                    white-space: nowrap;
                    padding: 0px;
                    margin: 4rem 0px;
                }

                .character {
                    display: inline-flex;
                    width: 3.5rem;
                    padding: 5px 10px;
                    background-color: #dadada;
                    border-radius: 20px;
                    margin: 0 3px;
                    justify-content: center;
                    align-items: center;
                }

                #copy-btn {
                    display: block;
                    border: 0;
                    background-color: #222751;
                    color: #f4f4f4;
                    font-size: 1.5rem;
                    font-family: cairo;
                    border-radius: 10px;
                    width: 100%;
                    padding-bottom: 5px;
                    cursor: pointer;
                }

                #data-footer {
                    justify-self: flex-end;
                }

                #email-container {
                    display: flex;
                    justify-content: center;
                    align-items: center;
                }

                #data {
                    flex-grow: 3;

                }

                #email-icon {
                    width: 30px;
                    margin-left: 20px;
                }

                #email-disc {
                    color: #222751;
                    margin-bottom: -7px;

                }

                #email-value {
                    color: #CA9228;
                    margin-top: -7px;
                }

                @media screen and (max-width: 840px) {
                    #data-container {
                        width: 100%;

                    }

                    #svg-container {
                        display: none;

                    }

                }
            </style>
        </head>

        <body>
            <div id='main-container'>
                <div id='data-container'>
                    <img id='qydha-logo-img'
                        src='https://storage.googleapis.com/qydha_bucket/qydha%20assets/EmailAssets/qydhaLogo.svg'
                        alt='Qydha Logo'>
                    <div id='data'>
                        <h1 id='header'>العب واحنا نقيدها</h1>
                        <p id='disc'>برجاء ادخال رمز الOTP لتاكيد البريد الالكترونى</p>
                        <p id='code' dir='ltr'>[code]</p>
                        <button id='copy-btn' onclick='copyToClipboard()'>نسخ الكود</button>

                    </div>
                    <div id='data-footer'>
                        <div id='email-container'>
                            <img id='email-icon'
                                src='https://storage.googleapis.com/qydha_bucket/qydha%20assets/EmailAssets/email_icon.svg'
                                alt='Email Icon'>
                            <div>
                                <p id='email-disc'>البريد الالكترونى</p>
                                <a id='email-value' href='mailto:customer-support@qydha.com'>customer-support@qydha.com</a>
                            </div>
                        </div>
                    </div>
                </div>
                <div id='svg-container'>
                    <img id='login-img' src='https://storage.googleapis.com/qydha_bucket/qydha%20assets/EmailAssets/x.svg'
                        alt=' Login Image'>
                </div>

            </div>
            <script>
                // Get the element with the id 'code'
                var codeElement = document.getElementById('code');

                // Get the text content of the element
                var codeText = codeElement.textContent;

                // Split the text into an array of characters
                var characters = codeText.split('');

                // Wrap each character in a span with a class for styling
                var styledText = characters.map(function (char) {
                    return '<span class='character'>' + char + '</span>';
                });

                // Set the innerHTML of the element to the styled text
                codeElement.innerHTML = styledText.join('');

                async function copyToClipboard() {
                    var copyText = [code];

                    var clipboardItem = new ClipboardItem({ 'text/plain': new Blob([copyText], { type: 'text/plain' }) });

                    try {
                        await navigator.clipboard.write([clipboardItem]);

                    } catch (err) {
                        console.error('Unable to copy to clipboard', err);
                    }
                }
            </script>
        </body>

        </html>
    ";
}
