<Ni xmlns="http://ns.renative.com/configuration/data"
				xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <!--增加-->
  <Command name="Test.Insert"  type="Text">
    <Content>
      <![CDATA[
     insert into Test (ID,Text) values ($Id1,$Text1);
	  ]]>
    </Content>
    <Int32 name ="Id1"/>
    <String name="Text1"/>
  </Command>
  <!--插入清除緩存-->
  <Command name="Test.Insert.Clear"  type="Text">
    <Content>
      <![CDATA[
      delete from Test<@cacheKey>;
	  ]]>
    </Content>
    <String name="cacheKey"/>
  </Command>
  <!--查询-->
  <Command name="Test.Select"  type="Text">
    <Content>
      <![CDATA[select * from Test;]]>
    </Content>
  </Command>
  <!--查询带缓存-->
  <Command name="Test.Select.Cache"  type="Text">
    <Content>
      <![CDATA[
      select * from Test <@cacheKey>;
	  ]]>
    </Content>
    <String name="cacheKey"/>
  </Command>
  <!--查询设置缓存-->
  <Command name="Test.Select.Set"  type="Text">
    <Content>
      <![CDATA[
      insert into Test<@cacheKey> (cacheKey,cacheValue) values (@cacheKey,@cacheValue);
	  ]]>
    </Content>
    <String name="cacheKey"/>
    <String name="cacheValue"/>
  </Command>
  <!--增加-->
  <Command name="NoSqlTest.Insert"  type="Text">
    <Content>
      <![CDATA[
     insert into NoSqlTest(Id,Name,Age) values ($Id1,$Name1,$Age1);
      insert into NoSqlTest(Id,Name,Age) values ($Id2,$Name2,$Age2);
	  ]]>
    </Content>
    <Int32 name ="Id1"/>
    <String name="Name1"/>
    <Int32 name ="Age1"/>
    <Int32 name ="Id2"/>
    <String name="Name2"/>
    <Int32 name ="Age2"/>
  </Command>
  <!--插入清除緩存-->
  <Command name="NoSqlTest.Insert.Clear"  type="Text">
    <Content>
      <![CDATA[
      delete from NoSqlTest<@cacheKey>;
	  ]]>
    </Content>
    <String name="cacheKey"/>
  </Command>
  <!--查询-->
  <Command name="NoSqlTest.Select"  type="Text">
    <Content>
      <![CDATA[
      select * from NoSqlTest;
	  ]]>
    </Content>
  </Command>
  <!--查询带缓存-->
  <Command name="NoSqlTest.Select.Cache"  type="Text">
    <Content>
      <![CDATA[
      select * from NoSqlTest<@cacheKey>;
	  ]]>
    </Content>
    <String name="cacheKey"/>
  </Command>

  <!--查询设置缓存-->
  <Command name="NoSqlTest.Select.Set"  type="Text">
    <Content>
      <![CDATA[
      insert into NoSqlTest<@cacheKey> (cacheKey,cacheValue) values (@cacheKey,@cacheValue);
	  ]]>
    </Content>
    <String name="cacheKey"/>
    <String name="cacheValue"/>
  </Command>

  <!--更新-->
  <Command name="NoSqlTest.Update"  type="Text">
    <Content>
      <![CDATA[
      update NoSqlTest set Name=$Name where Id=$Id;
	  ]]>
    </Content>
    <Int32 name="Id"/>
    <String name="Name"/>
  </Command>
  <!--更新清除緩存-->
  <Command name="NoSqlTest.Update.Clear"  type="Text">
    <Content>
      <![CDATA[
      delete from NoSqlTest;
	  ]]>
    </Content>
  </Command>



  <!--删除-->
  <Command name="NoSqlTest.Delete"  type="Text">
    <Content>
      <![CDATA[
      delete from NoSqlTest where Id = $Id;
	  ]]>
    </Content>
    <Int32 name="Id"/>
  </Command>
</Ni>
