
Imports Minecraft.Utils
Imports System.Collections.Generic
Imports System.IO
Imports fastJSON

Namespace Auth

	Public Class AuthData
		Private email As String = Nothing
		Private username As String = Nothing
		Private password As String = Nothing
		Private accessToken As String = Nothing
		Private clientToken As String = Nothing
		Private userID As String = Nothing
		Private profileID As String = Nothing
		Private twitchToken As String = Nothing
		Public Sub New(email As String, password As String)
			Me.email = email
			Me.password = password
		End Sub
		Public Sub New(response As AuthResponse)
			Me.accessToken = response.getAccessToken()
			Me.clientToken = response.getClientToken()
			Me.username = response.getUsername()
			Me.userID = response.getUserID()
			Me.twitchToken = response.getTwitchToken()
			Me.profileID = response.getProfileID()
		End Sub

		Public Sub New()
		End Sub
		Public Function getUsername() As String
			Return Me.username
		End Function
		Public Function getEmail() As String
			Return Me.email
		End Function
		Public Function getPassword() As String
			Return Me.password
		End Function
		Public Function getAccessToken() As String
			Return Me.accessToken
		End Function
		Public Function getClientToken() As String
			Return Me.clientToken
		End Function
		Public Function getUserID() As String
			Return Me.userID
		End Function
		Public Function getProfileID() As String
			Return Me.profileID
		End Function
		Public Function getTwitchToken() As String
			Return Me.twitchToken
		End Function
		Public Function loadData(file__1 As String) As [Boolean]
			Try
				Dim json__2 As String = File.ReadAllText(file__1)
				Dim parse As Dictionary(Of [String], [Object]) = DirectCast(JSON.Parse(json__2), Dictionary(Of [String], [Object]))
				For Each val As KeyValuePair(Of [String], [Object]) In parse
					Select Case val.Key
						Case "username"
							Me.username = Convert.ToString(val.Value)
							Exit Select
						Case "accessToken"
							Me.accessToken = Convert.ToString(val.Value)
							Exit Select
						Case "clientToken"
							Me.clientToken = Convert.ToString(val.Value)
							Exit Select
						Case "userID"
							Me.userID = Convert.ToString(val.Value)
							Exit Select
						Case "profileID"
							Me.profileID = Convert.ToString(val.Value)
							Exit Select
						Case "twitchToken"
							Me.twitchToken = Convert.ToString(val.Value)
							Exit Select
					End Select
				Next
				Return True
			Catch e As Exception
				Return False
			End Try
		End Function
		Public Function saveData(file__1 As String) As [Boolean]
			Try
				Dim data As New Dictionary(Of [String], [Object])()
				data.Add("username", Me.getUsername())
				data.Add("accessToken", Me.getAccessToken())
				data.Add("clientToken", Me.getClientToken())
				data.Add("userID", Me.getUserID())
				data.Add("profileID", Me.getProfileID())
				If twitchToken IsNot Nothing Then
					data.Add("twitchToken", Me.getTwitchToken())
				End If
				Dim json__2 As String = JSON.ToJSON(data)
				File.WriteAllText(file__1, json__2)
				Return True
			Catch e As Exception
				Return False
			End Try
		End Function
	End Class
End Namespace