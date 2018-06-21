using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace dotMailer.Api
{
    public partial class Client
    {
        private readonly HttpClient httpClient;
        private readonly MediaTypeFormatter jsonFormatter;

        public Client(string username, string password)
        {
            httpClient = GetHttpClient(username, password);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            jsonFormatter = new JsonMediaTypeFormatter { SerializerSettings = settings };
        }

        public Client(string username, string password, string baseUrl)
        {
            BaseAddress = baseUrl;
            httpClient = GetHttpClient(username, password);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            jsonFormatter = new JsonMediaTypeFormatter { SerializerSettings = settings };
        }

        #region Helpers

        private HttpClient GetHttpClient(string username, string password)
        {
            var client = new HttpClient { BaseAddress = new Uri(BaseAddress) };

            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64);

            return client;
        }

        private ServiceResult Result(HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode ? SuccessResult(response) : ErrorResult(response);
        }

        private ServiceResult<T> Result<T>(HttpResponseMessage response)
        {
            return response.IsSuccessStatusCode ? SuccessResult<T>(response) : ErrorResult<T>(response);
        }

        private ServiceResult SuccessResult(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsStringAsync().Result;
            return new ServiceResult(true, result);
        }

        private ServiceResult<T> SuccessResult<T>(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsAsync<T>(new[] { jsonFormatter }).Result;
            return new ServiceResult<T>(true, result);
        }

        private ServiceResult ErrorResult(HttpResponseMessage response)
        {
            var message = GetErrorMessage(response);
            return new ServiceResult(false, message);
        }

        private ServiceResult<T> ErrorResult<T>(HttpResponseMessage response)
        {
            var message = GetErrorMessage(response);
            return new ServiceResult<T>(false, default(T), message);
        }

        private ServiceResult ExceptionResult(Exception exception)
        {
            var message = GetExceptionMessage(exception);
            return new ServiceResult(false, message);
        }

        private ServiceResult<T> ExceptionResult<T>(Exception exception)
        {
            var message = GetExceptionMessage(exception);
            return new ServiceResult<T>(false, default(T), message);
        }

        private string GetErrorMessage(HttpResponseMessage response)
        {
            var errorInfo = response.Content.ReadAsAsync<RequestErrorInfo>().Result;
            var message = string.Format("Failed to {0} object (Status Code: {1}, Status Description: {2}, Detail: {3})", response.RequestMessage.Method.Method, (int)response.StatusCode, response.StatusCode, errorInfo.Message);
            return message;
        }

        private string GetExceptionMessage(Exception exception)
        {
            return string.Format("An exception occurred: {0}", exception);
        }

        #endregion

        #region Get

        private async Task<ServiceResult> GetAsync(Request request)
        {
            try
            {
                var response = await httpClient.GetAsync(request.Url);
                return Result(response);
            }
            catch (Exception exception)
            {
                return ExceptionResult(exception);
            }
        }

        private async Task<ServiceResult<T>> GetAsync<T>(Request request)
        {
            try
            {
                var response = await httpClient.GetAsync(request.Url);
                return Result<T>(response);
            }
            catch (Exception exception)
            {
                return ExceptionResult<T>(exception);
            }
        }

        #endregion

        #region Post

        private async Task<ServiceResult<T>> PostAsync<T>(Request request)
        {
            try
            {
                var response = await httpClient.PostAsync(request.Url, string.Empty, jsonFormatter);
                return Result<T>(response);
            }
            catch (Exception exception)
            {
                return ExceptionResult<T>(exception);
            }
        }

        private async Task<ServiceResult<T>> PostAsync<T>(Request request, T data)
        {
            try
            {
                var response = await httpClient.PostAsync(request.Url, data, jsonFormatter);
                return Result<T>(response);
            }
            catch (Exception exception)
            {
                return ExceptionResult<T>(exception);
            }
        }

        private async Task<ServiceResult<TOutput>> PostAsync<TOutput, TInput>(Request request, TInput data)
        {
            try
            {
                var response = await httpClient.PostAsync(request.Url, data, jsonFormatter);
                return Result<TOutput>(response);
            }
            catch (Exception exception)
            {
                return ExceptionResult<TOutput>(exception);
            }
        }

        #endregion

        #region Put

        private async Task<ServiceResult<T>> PutAsync<T>(Request request, T data)
        {
            try
            {
                var response = await httpClient.PutAsync(request.Url, data, jsonFormatter);
                return Result<T>(response);
            }
            catch (Exception exception)
            {
                return ExceptionResult<T>(exception);
            }
        }

        #endregion

        #region Delete

        private async Task<ServiceResult> DeleteAsync(Request request)
        {
            try
            {
                var response = await httpClient.DeleteAsync(request.Url);
                return Result(response);
            }
            catch (Exception exception)
            {
                return ExceptionResult(exception);
            }
        }

        private async Task<ServiceResult<T>> DeleteAsync<T>(Request request)
        {
            try
            {
                var response = await httpClient.DeleteAsync(request.Url);
                return Result<T>(response);
            }
            catch (Exception exception)
            {
                return ExceptionResult<T>(exception);
            }
        }

        #endregion
    }
}
