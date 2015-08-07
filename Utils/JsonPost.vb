Imports System.Net
Imports System.IO
Imports System.Text

Namespace Utils
	Public Class JsonPost
		Public Shared Function doPost(url As String, json As String) As String
			Dim data As Byte() = Encoding.UTF8.GetBytes(json)
			Dim request As WebRequest = WebRequest.Create(url)
			request.ContentType = "application/json; charset=utf-8"
			request.Method = "POST"
			request.ContentLength = data.Length
			Dim dataStream As Stream = request.GetRequestStream()
			dataStream.Write(data, 0, data.Length)
			dataStream.Close()
			Dim response As WebResponse = request.GetResponse()
			dataStream = response.GetResponseStream()
			Dim reader As New StreamReader(dataStream)
			Dim responseFromServer As String = reader.ReadToEnd()
			reader.Close()
			dataStream.Close()
			response.Close()
			Return responseFromServer
		End Function
	End Class
End Namespace