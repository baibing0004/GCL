<?xml version="1.0" encoding="utf-8" ?>
<Ni xmlns="http://ns.renative.com/configuration/data"
				xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <!--按首字母查询-->
  <Command name="Module.GetData"  type="Text" template="r_ac">
    <Content>
      <![CDATA[
          update ac_mail set a.DelFlag = 2,a.CaseLogID = ?ID
          where DelFlag = 0  LIMIT ?pageSize;
          select *,3 as sendrest from ac_mail a
          on a.DelFlag = 2 and a.CaseLogID = ?ID;
	    ]]>
    </Content>
    <String name="ID"/>
    <Int32 name="pageSize"/>
  </Command>
  <Command name="Module.finishMail"  type="Text" template="w_ac">
    <Content>
      <![CDATA[      
        INSERT INTO ac_maillog (MailID, SendDate, sendrest, CreateTime, DelFlag, Reasion) VALUES (?MailID,now(),?sendrest, now(),0,?reasion);
        update ac_mail a set a.DelFlag = 3
          where a.DelFlag = 2 and a.ID = ?MailID;
	    ]]>
    </Content>
    <String name="MailID"/>
    <String name="reasion"/>
    <Int32 name="sendrest"/>
  </Command>
  <Command name="Module.cancelMail"  type="Text" template="w_ac">
    <Content>
      <![CDATA[      
        INSERT INTO ac_maillog (MailID, SendDate, sendrest, CreateTime, DelFlag, Reasion) VALUES (?MailID,now(),?sendrest, now(),0,?reasion);
        update ac_mail a set a.DelFlag = 4
          where a.DelFlag = 2 and a.ID = ?MailID;
	    ]]>
    </Content>
    <String name="MailID"/>
    <String name="reasion"/>
    <Int32 name="sendrest"/>
  </Command>
  <Command name="Module.failMail"  type="Text" template="w_ac">
    <Content>
      <![CDATA[      
        INSERT INTO ac_maillog (MailID, SendDate, sendrest, CreateTime, DelFlag, Reasion) VALUES (?MailID,now(),?sendrest, now(),0,?reasion);
	    ]]>
    </Content>
    <String name="MailID"/>
    <String name="reasion"/>
    <Int32 name="sendrest"/>
  </Command>
  <Command name="Module.GetCaseFiles"  type="Text" template="w_ac">
    <Content>
      <![CDATA[      
        SELECT a.*,b.*,c.Content as Template
        FROM
          ac_case a
          inner join ac_document b
          on a.ReqDocID = b.ID or a.RecDocID = b.ID or a.ZTDocID = b.ID or a.ANDocID = b.ID or a.AnsDocID = b.ID or a.DNDocID = b.ID or a.DefenDocID = b.ID or a.ArbiDocID = b.ID
          inner join ac_template c
          on a.ID = ?DocID and a.templateID = b.ID and a.DelFlag = 0 and ifnull(?.User_UID,'')!='';
        SELECT c.*
        FROM
          ac_case a
          inner join ac_doccontent b
          on a.ReqDocID = b.DocumentID or a.RecDocID = b.DocumentID or a.ZTDocID = b.DocumentID or a.ANDocID = b.DocumentID or a.AnsDocID = b.DocumentID or a.DNDocID = b.DocumentID or a.DefenDocID = b.DocumentID or a.ArbiDocID = b.DocumentID
          inner join ac_content c
         on b.ContentID = c.ID and a.DelFlag = 0 and b.DelFlag = 0 and c.DelFlag = 0;
         update ac_case set CaseStatus = 8 where ID = ?CaseID;
	    ]]>
    </Content>
    <String name="ID"/>
  </Command>
  <Command name="Module.EndCase2"  type="Text" template="w_ac">
    <Content>
      <![CDATA[
        update ac_case set CaseStatus = 8 where ID = ?CaseID;
	    ]]>
    </Content>
    <String name="ID"/>
    <String name="CaseID"/>
    <String name="DocID"/>
    <String name="url"/>
    <String name="name"/>
  </Command>
</Ni>
