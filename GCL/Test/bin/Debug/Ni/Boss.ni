<?xml version="1.0" encoding="UTF-8"?>
<Ni>
  <Command name="AddBillDetail">
    <Content>usp_Boss_AddBillDetail</Content>
    <String name="@TransactionID" Size="32"/>
    <String name="@UserID" Size="32"/>
    <String name="@SPID" Size="32"/>
    <String name="@OrgID" Size="32"/>
    <String name="@CourseGroupID" Size="32"/>
    <String name="@CourseID" Size="32"/>
    <String name="@CourseWareID" Size="32"/>
    <DateTime name="@StampTime"/>
    <Int32 name="@Value"/>
    <Int32 name="@BillingType"/>
    <String name="@IPAddress" Size="64"/>
    <Int32 name="@BillingStyle"/>
  </Command>
  <Command name="AddLog">
    <Content>usp_Test_AddLog</Content>
    <String name="@text" Size="50"/>
  </Command>
</Ni>