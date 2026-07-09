using Microsoft.AspNetCore.Mvc;

namespace FileProcessor.Api.Controllers;

[ApiController]
[Route("test")]
public sealed class TestController : ControllerBase
{

    [HttpGet]
    public IActionResult Index()
    {
        var html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>FileProcessor API Test</title>
    <style>
        body { font-family: Arial, sans-serif; max-width: 900px; margin: 40px auto; background: #f5f5f5; }
        .container { background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
        h1 { color: #333; }
        .section { margin: 25px 0; padding: 20px; background: #f9f9f9; border-left: 4px solid #007bff; }
        input[type='text'], textarea { width: 100%; padding: 10px; margin: 8px 0; border: 1px solid #ddd; border-radius: 4px; box-sizing: border-box; }
        button { background: #007bff; color: white; padding: 10px 20px; border: none; border-radius: 4px; cursor: pointer; margin: 5px 0; }
        button:hover { background: #0056b3; }
        .output { background: #f0f0f0; padding: 15px; border-radius: 4px; margin-top: 10px; max-height: 300px; overflow-y: auto; font-family: monospace; white-space: pre-wrap; word-break: break-all; }
        .success { color: #28a745; }
        .error { color: #dc3545; }
        label { display: block; font-weight: bold; margin: 10px 0 5px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🚀 FileProcessor API Test</h1>
        <p>Use this page to test the API endpoints. Start by setting your API key below.</p>

        <div class='section'>
            <label>API Key (from X-API-Key header):</label>
            <input type='text' id='apiKey' value='local-test-key' placeholder='Enter your API key'>
            <button onclick='testConnection()'>Test Connection</button>
            <div id='connectionOutput' class='output' style='display:none;'></div>
        </div>

        <div class='section'>
            <h3>📊 Get File Report</h3>
            <p>Retrieve the report of processed files.</p>
            <button onclick='getReport()'>Get Report</button>
            <div id='reportOutput' class='output' style='display:none;'></div>
        </div>

        <div class='section'>
            <h3>📤 Upload CSV File</h3>
            <p>Upload a CSV file with columns: Name, Age, Salary</p>
            <label>Select CSV File:</label>
            <input type='file' id='csvFile' accept='.csv'>
            <button onclick='uploadFile()'>Upload File</button>
            <div id='uploadOutput' class='output' style='display:none;'></div>
        </div>

        <div class='section'>
            <h3>📄 OpenAPI Specification</h3>
            <p>Download or view the OpenAPI/Swagger specification.</p>
            <button onclick='getOpenApiSpec()'>Fetch OpenAPI Spec</button>
            <div id='openApiOutput' class='output' style='display:none;'></div>
        </div>
    </div>

    <script>
        const baseUrl = window.location.origin;

        function getApiKey() {
            return document.getElementById('apiKey').value || 'local-test-key';
        }

        function showOutput(elementId, message, isError = false) {
            const el = document.getElementById(elementId);
            el.textContent = message;
            el.style.display = 'block';
            el.className = 'output ' + (isError ? 'error' : 'success');
        }

        async function testConnection() {
            try {
                showOutput('connectionOutput', 'Testing connection...');
                const response = await fetch(baseUrl + '/api/files/report', {
                    headers: { 'X-API-Key': getApiKey() }
                });
                if (response.ok) {
                    showOutput('connectionOutput', '✓ Connection successful! API key is valid.');
                } else {
                    showOutput('connectionOutput', '✗ Connection failed: ' + response.statusText, true);
                }
            } catch (e) {
                showOutput('connectionOutput', '✗ Error: ' + e.message, true);
            }
        }

        async function getReport() {
            try {
                showOutput('reportOutput', 'Fetching report...');
                const response = await fetch(baseUrl + '/api/files/report', {
                    headers: { 'X-API-Key': getApiKey() }
                });
                const data = await response.json();
                showOutput('reportOutput', JSON.stringify(data, null, 2));
            } catch (e) {
                showOutput('reportOutput', '✗ Error: ' + e.message, true);
            }
        }

        async function uploadFile() {
            const fileInput = document.getElementById('csvFile');
            if (!fileInput.files.length) {
                showOutput('uploadOutput', '✗ Please select a file', true);
                return;
            }
            try {
                showOutput('uploadOutput', 'Uploading...');
                const formData = new FormData();
                formData.append('file', fileInput.files[0]);
                const response = await fetch(baseUrl + '/api/files/upload', {
                    method: 'POST',
                    headers: { 'X-API-Key': getApiKey() },
                    body: formData
                });
                const data = await response.json();
                showOutput('uploadOutput', JSON.stringify(data, null, 2));
            } catch (e) {
                showOutput('uploadOutput', '✗ Error: ' + e.message, true);
            }
        }

        async function getOpenApiSpec() {
            try {
                showOutput('openApiOutput', 'Fetching OpenAPI spec...');
                const response = await fetch(baseUrl + '/openapi/spec.json');
                const data = await response.json();
                showOutput('openApiOutput', JSON.stringify(data, null, 2));
            } catch (e) {
                showOutput('openApiOutput', '✗ Error: ' + e.message, true);
            }
        }
    </script>
</body>
</html>
";
        return Content(html, "text/html; charset=utf-8");
    }
}
