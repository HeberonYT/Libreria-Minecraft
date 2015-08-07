Imports System.Collections.Generic
Imports fastJSON

Namespace Auth

	Public Class AuthService
		Public Shared Function authenticate(d As AuthData) As AuthResponse
			Try
				If d.getEmail() Is Nothing OrElse d.getPassword() Is Nothing Then
					Return New AuthResponse(True)
				End If
				Dim dic As New Dictionary(Of [String], [Object])()
				Dim agent As New Dictionary(Of [String], [Object])()
				agent.Add("name", "Minecraft")
				agent.Add("version", 1)
				dic.Add("agent", agent)
				dic.Add("username", d.getEmail())
				dic.Add("password", d.getPassword())
				dic.Add("requestUser", True)
				Dim request As String = JSON.ToJSON(dic)
				Dim response As String = Minecraft.Utils.JsonPost.doPost("https://authserver.mojang.com/authenticate", request)
				If response Is Nothing OrElse response.Length = 0 OrElse response.Contains("UserMigratedException") OrElse response.Contains("ForbiddenOperationException") Then
					Return New AuthResponse(True)
				End If
				Dim parse As Dictionary(Of [String], [Object]) = DirectCast(JSON.Parse(response), Dictionary(Of [String], [Object]))
				Dim accessToken As String = Nothing
				Dim clientToken As String = Nothing
				Dim username As String = Nothing
				Dim userID As String = Nothing
				Dim profileID As String = Nothing
				Dim twitchToken As String = Nothing
				For Each val As KeyValuePair(Of [String], [Object]) In parse
					Select Case val.Key
						Case "accessToken"
							accessToken = Convert.ToString(val.Value)
							Exit Select
						Case "clientToken"
							clientToken = Convert.ToString(val.Value)
							Exit Select
						Case "selectedProfile"
							Dim sp As Dictionary(Of [String], [Object]) = DirectCast(val.Value, Dictionary(Of [String], [Object]))
							For Each val2 As KeyValuePair(Of [String], [Object]) In sp
								Select Case val2.Key
									Case "id"
										profileID = Convert.ToString(val2.Value)
										Exit Select
									Case "name"
										username = Convert.ToString(val2.Value)
										Exit Select
								End Select
							Next
							Exit Select
						Case "user"
							Dim sp2 As Dictionary(Of [String], [Object]) = DirectCast(val.Value, Dictionary(Of [String], [Object]))
							For Each val3 As KeyValuePair(Of [String], [Object]) In sp2
								Select Case val3.Key
									Case "id"
										userID = Convert.ToString(val3.Value)
										Exit Select
									Case "properties"
										Dim list As List(Of [Object]) = DirectCast(val3.Value, List(Of [Object]))

										For i As Integer = 0 To list.Count - 1
											For Each val4 As KeyValuePair(Of [String], [Object]) In DirectCast(list(i), Dictionary(Of [String], [Object]))
												If val4.Key.Equals("value") AndAlso i = 0 Then
													twitchToken = Convert.ToString(val4.Value)
												End If
											Next
										Next
										Exit Select
								End Select
							Next
							Exit Select
					End Select
				Next
				If accessToken Is Nothing OrElse clientToken Is Nothing OrElse username Is Nothing OrElse userID Is Nothing OrElse profileID Is Nothing Then
					Return New AuthResponse(True)
				End If
				Return New AuthResponse(accessToken, clientToken, username, userID, profileID, twitchToken, False)
			Catch e As Exception
				Return New AuthResponse(True)
			End Try
		End Function
		Public Shared Function refresh(d As AuthData) As AuthResponse
			Try
				If d.getAccessToken() Is Nothing OrElse d.getClientToken() Is Nothing Then
					Return New AuthResponse(True)
				End If
				Dim dic As New Dictionary(Of [String], [Object])()
				dic.Add("accessToken", d.getAccessToken())
				dic.Add("clientToken", d.getClientToken())
				dic.Add("requestUser", True)
				Dim request As String = JSON.ToJSON(dic)
				Dim response As String = Minecraft.Utils.JsonPost.doPost("https://authserver.mojang.com/refresh", request)
				If response Is Nothing OrElse response.Length = 0 OrElse response.Contains("ForbiddenOperationException") Then
					Return New AuthResponse(True)
				End If
				Dim parse As Dictionary(Of [String], [Object]) = DirectCast(JSON.Parse(response), Dictionary(Of [String], [Object]))
				Dim accessToken As String = Nothing
				Dim clientToken As String = Nothing
				Dim username As String = Nothing
				Dim userID As String = Nothing
				Dim profileID As String = Nothing
				Dim twitchToken As String = Nothing
				For Each val As KeyValuePair(Of [String], [Object]) In parse
					Select Case val.Key
						Case "accessToken"
							accessToken = Convert.ToString(val.Value)
							Exit Select
						Case "clientToken"
							clientToken = Convert.ToString(val.Value)
							Exit Select
						Case "selectedProfile"
							Dim sp As Dictionary(Of [String], [Object]) = DirectCast(val.Value, Dictionary(Of [String], [Object]))
							For Each val2 As KeyValuePair(Of [String], [Object]) In sp
								Select Case val2.Key
									Case "id"
										profileID = Convert.ToString(val2.Value)
										Exit Select
									Case "name"
										username = Convert.ToString(val2.Value)
										Exit Select
								End Select
							Next
							Exit Select
						Case "user"
							Dim sp2 As Dictionary(Of [String], [Object]) = DirectCast(val.Value, Dictionary(Of [String], [Object]))
							For Each val3 As KeyValuePair(Of [String], [Object]) In sp2
								Select Case val3.Key
									Case "id"
										userID = Convert.ToString(val3.Value)
										Exit Select
									Case "properties"
										Dim list As List(Of [Object]) = DirectCast(val3.Value, List(Of [Object]))

										For i As Integer = 0 To list.Count - 1
											For Each val4 As KeyValuePair(Of [String], [Object]) In DirectCast(list(i), Dictionary(Of [String], [Object]))
												If val4.Key.Equals("value") AndAlso i = 0 Then
													twitchToken = Convert.ToString(val4.Value)
												End If
											Next
										Next
										Exit Select
								End Select
							Next
							Exit Select
					End Select
				Next
				If accessToken Is Nothing OrElse clientToken Is Nothing OrElse username Is Nothing OrElse userID Is Nothing OrElse profileID Is Nothing Then
					Return New AuthResponse(True)
				End If
				Return New AuthResponse(accessToken, clientToken, username, userID, profileID, twitchToken, _
					False)
			Catch e As Exception
				Return New AuthResponse(True)
			End Try
		End Function
		Public Shared Function validate(d As AuthData) As AuthResponse
			Try
				If d.getAccessToken() Is Nothing OrElse d.getClientToken() Is Nothing Then
					Return New AuthResponse(True)
				End If
				Dim dic As New Dictionary(Of [String], [Object])()
				dic.Add("accessToken", d.getAccessToken())
				dic.Add("clientToken", d.getClientToken())
				Dim request As String = JSON.ToJSON(dic)
				Dim response As String = Minecraft.Utils.JsonPost.doPost("https://authserver.mojang.com/validate", request)
				If response Is Nothing OrElse response.Length <> 0 Then
					Return New AuthResponse(True)
				End If
				Return New AuthResponse(False)
			Catch e As Exception
				Return New AuthResponse(True)
			End Try
		End Function
	End Class
End Namespace