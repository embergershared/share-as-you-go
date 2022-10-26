// <copyright file="IServiceBusClient.cs" company="PlaceholderCompany">
//
// DISCLAIMER
//
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// </copyright>

using Azure.Messaging.ServiceBus;

namespace SecureSBClient.Interfaces
{
    internal interface IServiceBusClient
    {
        bool CreateClient(string sbNamespace, string? clientId = null);
        Task DisposeClientAsync();
        Task<bool> SendMessageAsync(string queue, string message);
        Task<ServiceBusReceivedMessage?> ReceiveMessageAsync(string queue);
        Task<bool> DeleteAllMessagesAsync(string queue);
    }
}
