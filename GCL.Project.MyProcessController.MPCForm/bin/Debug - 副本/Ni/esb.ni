<Config>
  <Ni xmlns="http://tempuri.org/ni.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema_instance">
    <Command name="_back" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[#存储过程名]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"></String>
      <String xmlns="" name="@_SessionID" Size="256"></String>
      <String xmlns="" name="@_SerID" Size="32"></String>
      <String xmlns="" name="@_EventID" Size="32"/>
      <String xmlns="" name="@_EventName" Size="256"></String>
      <Int32 xmlns="" name="@_EventLevel"/>
      <String xmlns="" name="@_EventVer" Size="11"></String>
      <DateTime xmlns="" name="@_CreateTime"/>
      <DateTime xmlns="" name="@_ExpireTime"/>
      <String xmlns="" name="@_MessageVer" Size="11"></String>
      <String xmlns="" name="@_Datas" Size="60000"></String>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.DBCache.Set" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_CacheSet]]>
      </Content>
      <String xmlns="" name="@_Key" Size="128"></String>
      <String xmlns="" name="@_Value" Size="256"></String>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.DBCache.Get" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_CacheGet]]>
      </Content>
      <String xmlns="" name="@_Key" Size="128"></String>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.DBCache.Remove" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_CacheRemove]]>
      </Content>
      <String xmlns="" name="@_Key" Size="128"></String>
    </Command>
    
    <Command name="PublicClass.Project.ESB.Module.DealProcess.PreDealHistory" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_PreDealResult]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <Int32 xmlns="" name="@_state" Size="11"/>
      <String xmlns="" name="@_stuts" Size="256"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.DealProcess.DealHistory" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_DealResult]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <Int32 xmlns="" name="@_state" Size="11"/>
      <String xmlns="" name="@_stuts" Size="256"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.DealProcess.Deal" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_DealEvent]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <String xmlns="" name="@_SubIDs" Size="5000"/>
      <String xmlns="" name="@_ThreadID" Size="32"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.DealProcess.ReDeal" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_AddReDeal]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
    </Command>
    
    <Command name="PublicClass.Project.ESB.Module.SendProcess.PreSend" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_PostEvent]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <String xmlns="" name="@_ThreadID" Size="32"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.SendProcess.PreSendHistory" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_PrePostResult]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <Int32 xmlns="" name="@_state" Size="11"/>
      <String xmlns="" name="@_stuts" Size="256"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.SendProcess.SendHistory" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_PostResult]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <Int32 xmlns="" name="@_state" Size="11"/>
      <String xmlns="" name="@_stuts" Size="256"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.SendProcess.RePost" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_AddRePost]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
    </Command>

    <Command name="PublicClass.Project.ESB.Module.DBReDealProcess.GetReDealData" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_GetReDealEvent]]>
      </Content>
      <Int32 xmlns="" name ="_Size"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.ReDealProcess.DealHistory" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_DealResult]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <Int32 xmlns="" name="@_state" Size="11"/>
      <String xmlns="" name="@_stuts" Size="256"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.ReDealProcess.Deal" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_ReDealEvent2]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <String xmlns="" name="@_SubIDs" Size="5000"/>
      <String xmlns="" name="@_ThreadID" Size="32"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.EndDealProcess.DealHistory" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_DealResult]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <Int32 xmlns="" name="@_state" Size="11"/>
      <String xmlns="" name="@_stuts" Size="256"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.EndDealProcess.Deal" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_ReDealEvent2]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <String xmlns="" name="@_SubIDs" Size="5000"/>
      <String xmlns="" name="@_ThreadID" Size="32"/>
    </Command>

    <Command name="PublicClass.Project.ESB.Module.DBReSendProcess.GetReSendData" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_GetRePostEvent]]>
      </Content>
      <Int32 xmlns="" name ="_Size"/>
    </Command>    
    <Command name="PublicClass.Project.ESB.Module.ReSendProcess.PreSend" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_RePostEvent2]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <String xmlns="" name="@_ThreadID" Size="32"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.ReSendProcess.SendHistory" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_PostResult]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <Int32 xmlns="" name="@_state" Size="11"/>
      <String xmlns="" name="@_stuts" Size="256"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.EndSendProcess.PreSend" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_RePostEvent2]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <String xmlns="" name="@_ThreadID" Size="32"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.EndSendProcess.SendHistory" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_PostResult]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <Int32 xmlns="" name="@_state" Size="11"/>
      <String xmlns="" name="@_stuts" Size="256"/>
    </Command>
    
    <Command name="PublicClass.Project.ESB.Module.DBFindEventMethodFactory.GetFindEventMethod" type="Text">
      <Content xmlns="">
        <![CDATA[SELECT * FROM esb_EventSubscription WHERE State IN (2, 3);]]>
      </Content>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.DBSignMethodFactory.GetSignMethodValue" type="Text">
      <Content xmlns="">
        <![CDATA[SELECT SignObjID,sKey,sIV FROM esb_EndPoint where ID = ?_ID;]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
    </Command>
    
    <Command name="PublicClass.Project.ESB.Module.DBOnLineProcess2.GetOffLine2" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_GetOffLine2]]>
      </Content>
      <Int32 xmlns="" name ="_Size"/>
    </Command>
    <Command name="PublicClass.Project.ESB.Module.DBOnLineProcess2.SendHistory" type="StoredProcedure">
      <Content xmlns="">
        <![CDATA[esb_usp_PostResult]]>
      </Content>
      <String xmlns="" name="@_ID" Size="32"/>
      <Int32 xmlns="" name="@_state" Size="11"/>
      <String xmlns="" name="@_stuts" Size="256"/>
    </Command>
  </Ni>
</Config>
