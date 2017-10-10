<Config>
  <Ni xmlns="http://tempuri.org/ni.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema_instance">
    <Command name="VESHTest.Module.JS.Tree" type="Text">
      <Content xmlns=""><![CDATA[
		SELECT a.CategorySN AS id
			 , a.DisplayName AS text
			 , case ifnull(b.CategorySN, '') when '' then 'open' else 'closed' end AS state
		FROM
		  T_Category a
		LEFT JOIN T_Category b
		ON a.CategorySN = b.ParentKey
		  where a.ParentKey = ifnull(?_ParentID,0)
		GROUP BY
		  a.CategorySN
		, a.DisplayName;
	  ]]></Content>
      <String xmlns="" name="@_ParentID" Size="32"></String>
    </Command>
    <Command name="PublicClass.Project.VESH.E.Entity.Power.Permission.AS.ASPermissionAction.Permission" type="Text">
      <Content><![CDATA[select * from SSIDB_Permission where DelFlag = 0;]]></Content>
    </Command>
    <Command name="PublicClass.Project.VESH.E.Entity.Power.Permission.PermissionModuler.XAuPermission" type="Text">
      <Content><![CDATA[select * from ssidb_XAuPermission;]]></Content>
    </Command>
    <Command name="PublicClass.Project.VESH.E.Entity.Power.Security.OnceSecurity.AddOnceUser">
      <Content><![CDATA[SSIDB_AddOnceUser]]></Content>
      <String name="@UserID" Size="32"></String>
      <String name="@Code" Size="64"></String>
    </Command>
    <Command name="PublicClass.Project.VESH.E.Entity.Power.Security.OnceSecurity.CheckOnceUser">
      <Content><![CDATA[SSIDB_CheckOnceUser]]></Content>
      <String name="@UserID" Size="32"></String>
      <String name="@Code" Size="64"></String>
    </Command>
    <Command name="PublicClass.Project.VESH.E.Entity.Power.Security.OnceSecurity.RemoveOnceUser">
      <Content><![CDATA[SSIDB_RemoveOnceUser]]></Content>
      <String name="@UserID" Size="32"></String>
    </Command>    
    <Command name="v2.GetMenu">
      <Content><![CDATA[ssidb_USP_GetMenu]]></Content>
      <String name="@PerIDs" Size="2048"></String>
      <String name="@SystemID" Size="32"></String>
    </Command>
    <Command name="v2.GetRight" type="Text">
      <Content><![CDATA[select PerID from ssidb_XAuPermission where Path = @Path;]]></Content>
      <String name="@Path" Size="500"></String>
    </Command>
    <Command name="v2.CheckRight" type="Text">
      <Content>
        <![CDATA[
          declare @t table(ID varchar(32) COLLATE Chinese_PRC_CS_AS);
          insert into @t
          select ID from dbo.ssidb_USF_GetC1TableFromString(@PIDS);
          select count(1) from @t a inner join ssidb_XAuPermission (nolock) b
          on a.ID = b.PerID and b.code = @code
          right join ssidb_XAuPermission (nolock) c
          on a.PerID = c.PerID and c.code = @code
          where a.ID is null;
        ]]>
      </Content>
      <String name="@Path" Size="500"></String>
      <String name="@PIDS" Size="500"></String>
      <Int32 name="@code"></Int32>
    </Command>
    <Command name="v2.CheckRight1" type="Text">
      <Content>
        <![CDATA[
          declare @t table(ID varchar(32) COLLATE Chinese_PRC_CS_AS);
          insert into @t
          select ID from dbo.ssidb_USF_GetC1TableFromString(@PIDS);
          select count(1) from ssidb_XAuPermission (nolock) a left join @t b
          on a.PerID = b.ID
          where charindex(a.[Path],@Path)>0 and b.ID is null;
        ]]>
      </Content>
      <String name="@Path" Size="500"></String>
      <String name="@PIDS" Size="500"></String>
    </Command>
    <Command name="PublicClass.Project.VESH.E.Entity.MLang.MLDBResource.GetMLangPackage" type="text">
      <Content><![CDATA[
          select * from ssidb_MLang where Mlang = @MLang and PackageID = @PackageID;
      ]]></Content>
      <String name="@PackageID" size ="50"></String>
      <String name="@MLang" size ="32"></String>
    </Command>
  </Ni>
</Config>
