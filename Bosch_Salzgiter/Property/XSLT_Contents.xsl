<xsl:stylesheet version="1.0"
      xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      xmlns:ms="urn:schemas-microsoft-com:xslt"
      xmlns:dt="urn:schemas-microsoft-com:datatypes"
	    xmlns:cs="urn:scripts-csharp"
      xmlns:extobj ="urn:extension-object">
  <!-- output -->
<xsl:output method="text" version="1.0"/>

  <!-- script -->
  <ms:script language="C#" implements-prefix="cs">
  <![CDATA[

  ]]>
  </ms:script>
  <!-- variables   -->
  <xsl:variable name="delimiter" select="','"/>
  <xsl:variable name="newline" select="'&#xA;'" />
  
<xsl:variable name="columns">
	<column>PCBGUID</column>
	<column>Name</column>
  	<column>X Distance</column>
	<column>Y Distance</column>
	<column>XY Distance</column>
</xsl:variable>

  <!-- params -->
  <xsl:param name="DestPath"/>
  <xsl:param name="FileName"/>
  <xsl:param name="Sequence"/>
<xsl:template match="/">
  
  <!-- Insert here -->
  <xsl:for-each select="ms:node-set($columns)/column">
  <xsl:value-of select="."/>
	  <xsl:if test="position() != last()">
		  <xsl:value-of select="$delimiter"/>
	  </xsl:if>
</xsl:for-each>
<xsl:value-of select="$newline"/>

  <xsl:for-each select="InspectResult/PCB/PanelGroup/Panel/Component/Defect[@InspTypeName='CriticalDistance']">
  		<xsl:value-of select="../../../../@PCBGUID"/>
  		<xsl:value-of select="$delimiter"/>
  		<xsl:value-of select="../@uname"/>
  		<xsl:value-of select="$delimiter"/>
  		<xsl:value-of select="@Value3 div 1000"/>
  		<xsl:value-of select="$delimiter"/>
  		<xsl:value-of select="@Value1 div 1000"/>
  		<xsl:value-of select="$delimiter"/>
  		<xsl:value-of select="@Value2 div 1000"/>
  		<xsl:if test="position() != last()">
		  <xsl:value-of select="$newline"/>
	  	</xsl:if>
  </xsl:for-each>
  
</xsl:template>

</xsl:stylesheet>