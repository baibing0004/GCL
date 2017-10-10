<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="yes"/>
  <xsl:template match="/">
    哈哈 让我们看看Second的值是什么<xsl:apply-templates select="first"/> 收到
    <a></a> html与text 对处理结果不同
    <![CDATA[请注意这里的<a></a>与XML中不同
    <a>heihei</a>]]>
    <xsl:apply-templates select="//five"/>
    第二个模式： 使用参数时 with-param节点必须是子节点而不是兄弟节点 Sort节点是另类 而且参数语境要注意 现在的语境还是/状态
    所以 &lt;xls:with-param name="para1" select="//*[@a1]" /&gt; 与 *[@a1]相同
    <xsl:apply-templates select="//five" mode="a">    
    	<xsl:with-param name="para1" select="//*[@a1]" />
    </xsl:apply-templates>
  </xsl:template>
  <xsl:template match="first">
  <xsl:for-each select="second">
    <xsl:value-of select="."/>
    <xsl:value-of select="."/>
  </xsl:for-each>
  </xsl:template>
  <xsl:template match="five">
  	<xsl:for-each select="q1|q2|q3|q4">
  	
  	<xsl:if test=".=20">
  	==20
  	</xsl:if>
  	
  	<xsl:if test=".&lt;=20">
  	&lt;=20
  	</xsl:if>
  	
  	<xsl:value-of select="." disable-output-escaping = "yes" /> 
  	属性模版：在括号内使用属性时使用{}框起来 但是在括号外可以不这样使用：
  	<xsl:if test="@a1">
  	a1= &quot;<xsl:value-of select="@a1"/>&quot;  	
  	<img src="{./@a1}" /> 两者相等 <img src="{@a1}" /> 但不等于 <img src="@a1" /> 
  	数字测试
  	<xsl:number value="position()" format="1. "/> 不相等 <xsl:number/>
  	</xsl:if>  	
  	</xsl:for-each>
  </xsl:template>
  <xsl:template match="five" mode="a">
  	<xsl:param name="para1" select="."></xsl:param>  	
  	<xsl:for-each select="q1|q2|q3|q4">
  	<xsl:sort select="." order="descending"/>  	
  	在for-each或者apply-templates后面进行排序
  	<xsl:copy/>
  	<xsl:value-of select="."/> a1= &quot;<xsl:value-of select="@a1"/>&quot; 	
  	<xsl:choose>
  	<xsl:when test=".=20">
  	等于20
  	</xsl:when>
  	<xsl:when test=".&lt;20">
  	小于20
  	</xsl:when>
  	<xsl:otherwise>
  	一般般
  	</xsl:otherwise>
  	</xsl:choose>  		  	
  	</xsl:for-each>
  	Copy使用方法
  	<xsl:copy/>
  	Copy-of使用方法
  	<xsl:copy-of select="q1|q2|q3|q4"/>
  	<xsl:variable name="n" select="q1"/>
  	Copy-of n:
  	<xsl:copy-of select="$n"/><xsl:text>
  	</xsl:text>
  	Copy-of n2:
  	<xsl:variable name="n2" select="*[@a1]"/>
  	<xsl:copy-of select="$n2"/>
  	Copy-of para1:
  	<xsl:copy-of select="$para1"/>
  	<xsl:value-of select="namespace-uri()"/>
  </xsl:template>
</xsl:stylesheet>