VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Form_Distribute Ground Form"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False


Option Compare Database
Option Explicit

Private Sub AddAccount_AfterUpdate()
Dim SQL As String
SQL = "INSERT INTO [Temp Account List] ( [Account ID] ) SELECT [Account List].[Account ID] FROM [Account List] "
SQL = SQL & " WHERE [Account List].[Account ID]  Like '" & Me![AddAccount] & "*';"
DoCmd.RunSQL SQL
Me![AccountList].Requery
Me![Distribute Criteria Form].Form.Requery
End Sub

Private Sub Close_Click()
DoCmd.Close
End Sub

Private Sub Distribute_Click()
On Error Resume Next
Dim MyRecord  As DAO.Recordset
Dim MyDB As DAO.Database
Dim ExRecord As DAO.Recordset
Set MyDB = CurrentDb
'Fill Bill Table
Set MyRecord = MyDB.OpenRecordset("Bills")
MyRecord.AddNew
MyRecord("Bill Date") = Me.End_Date
MyRecord("Contents") = getmessage("051") & Me.SrAccountID
MyRecord("Bill No") = DMax("[Bill ID]", "[Bills]") + 1
MyRecord("BookFlag") = True
MyRecord.Update
gBillID = DMax("[Bill ID]", "[Bills]")

Set MyRecord = MyDB.OpenRecordset("Transaction List")
 
 MyRecord.AddNew
 MyRecord("Bill ID") = gBillID
 MyRecord("Amount") = Me![Total]
 MyRecord("Account ID") = Me![SrAccountID]
 MyRecord("DBCR") = -1
 'them code thinhpv
 MyRecord("CreateDate") = Now
 MyRecord("CreateUser") = CurrentUser
 'end code thinhpv
 MyRecord.Update
 
 gID = DMax("[ID]", "[Transaction List]")
 
MyRecord.AddNew
 MyRecord("Bill ID") = gBillID
 MyRecord("Amount") = Me![Total]
 MyRecord("Account ID") = Me![DesAccountID]
 MyRecord("DBCR") = 1
 MyRecord("ParrentID") = gID
 'them code thinhpv
 MyRecord("CreateDate") = Now
 MyRecord("CreateUser") = CurrentUser
 'end code thinhpv
 MyRecord.Update
 MyRecord.Close

' Details of Credit Expense  Account (Source Account)
 
 Set MyRecord = Me![Distribute Expense SubForm].Form.RecordsetClone
 Set ExRecord = MyDB.OpenRecordset("Expense Object Input Line")
 MyRecord.MoveFirst
 Do While Not MyRecord.EOF
        ExRecord.AddNew
        ExRecord("ID") = gID
        ExRecord("ExObject ID") = MyRecord("ExObject ID")
        ExRecord("Expense Atribute ID") = MyRecord("Expense Atribute ID")
        ExRecord("Amount") = MyRecord("Amount")
        ExRecord("TransFlag") = True
        ExRecord.Update
        MyRecord.MoveNext
     Loop
MyRecord.Close
ExRecord.Close

Set MyRecord = Me![Distribute Criteria Form].Form.RecordsetClone
Set ExRecord = MyDB.OpenRecordset("Expense Object Input Line")
 MyRecord.MoveFirst
 Do While Not MyRecord.EOF
        ExRecord.AddNew
        ExRecord("ID") = gID + 1
        ExRecord("ExObject ID") = MyRecord("ExObject ID")
        ExRecord("Expense Atribute ID") = Me![ExAtribute ID]
        ExRecord("Amount") = (MyRecord("Amount") / Me![Distribute Criteria Form].Form![Total]) * Me![Total]
        ExRecord.Update
  MyRecord.MoveNext

    Loop
MyRecord.Close
ExRecord.Close
msgboxOK (getmessage("020") & " " & Me![SrAccountID] & getmessage("021") & " " & Me![DesAccountID] & getmessage("022"))
'msgbox "Qu¸ tr×nh ph©n bæ  tõ tµi kho¶n " & Me![SrAccountID] & " sang tµi kho¶n " & Me![DesAccountID] & " ®· kÕt thóc"

Me.Refresh
End Sub

Private Sub Form_Load()
On Error Resume Next
DoCmd.SetWarnings False
Me![SrAccountID] = GetSetting(gApplicationName, "Expense Distribution", "SrAccountID", "")
Me![DesAccountID] = GetSetting(gApplicationName, "Expense Distribution", "DesAccountID", "")
Me![ExAtribute ID] = GetSetting(gApplicationName, "Expense Distribution", "Expense Attribute ID", "")
Me![Begin Date] = GetSetting(gApplicationName, "Expense Distribution", "Begin Date", Date)
Me![End Date] = GetSetting(gApplicationName, "Expense Distribution", "End Date", Date)
Me![End Date] = GetSetting(gApplicationName, "Expense Distribution", "End Date", Date)
End Sub

Private Sub Form_Unload(Cancel As Integer)
Dim i As Integer
Dim temp As Variant
Call SaveSetting(gApplicationName, "Expense Distribution", "SrAccountID", Me![SrAccountID])
Call SaveSetting(gApplicationName, "Expense Distribution", "DesAccountID", Me![DesAccountID])
Call SaveSetting(gApplicationName, "Expense Distribution", "Expense Attribute ID", Me![ExAtribute ID])
Call SaveSetting(gApplicationName, "Expense Distribution", "Begin Date", Me![Begin Date])
Call SaveSetting(gApplicationName, "Expense Distribution", "End Date", Me![End Date])
For i = 0 To Me![AccountList].ListCount - 1
 If i <> 0 Then
 temp = temp & "," & Me.AccountList.ItemData(i)
 Else
 temp = temp & Me.AccountList.ItemData(i)
 End If
Next i
Call SaveSetting(gApplicationName, "Expense Distribution", "Criteria", temp)
End Sub

Private Sub print_Click()
DoCmd.OpenReport "Distribution Report", acViewPreview
End Sub

Private Sub Refresh_Click()
Me.Refresh
End Sub

Private Sub Remove_Click()
Dim SQL
SQL = "Delete * from [Temp Account List] Where [Account ID]='" & Me![AccountList] & "'"
DoCmd.RunSQL SQL
Me![AccountList].Requery
Me![Distribute Criteria Form].Requery
Me![Distribute Expense SubForm].Requery
End Sub

Private Sub RemoveAll_Click()
Dim SQL As String
SQL = "Delete * from [Temp Account List]"
DoCmd.RunSQL SQL
Me![AccountList].Requery
DoCmd.RunCommand acCmdSaveRecord
Me![Distribute Criteria Form].Form.Requery


End Sub


Private Sub Review_Enter()
Dim m
m = SaveRecord()
gTrackingQuery = "Expense Transfer Tracking Query"
'DoCmd.OpenForm "Transfer Tracking Form"
Call OpenAnyForm("Transfer Tracking Form", "", A_EDIT)
End Sub

Private Sub SrAccountID_AfterUpdate()
Dim m
m = SaveRecord()
Me![Distribute Expense SubForm].Form.Requery
Me![Distribute Criteria Form].Form.Requery
End Sub

