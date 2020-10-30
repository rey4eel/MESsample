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

  <!-- params -->
  <xsl:param name="DestPath"/>
  <xsl:param name="Sequence"/>
  <xsl:param name="FileExportDestPath"/>
  
  <xsl:template match="/">
    
    <!-- Insert here -->
    <xsl:value-of select="$FileExportDestPath"/>
     <xsl:text>\</xsl:text>
      <xsl:choose>
		<xsl:when test="InspectResult/PCB/@Lot=''">
		    <xsl:value-of select="InspectResult/PCB/@BarCode"/>
		</xsl:when>
		<xsl:otherwise>
		    <xsl:value-of select="InspectResult/PCB/@Lot"/>
		</xsl:otherwise>
	</xsl:choose>

    <xsl:text>.csv</xsl:text>
    
  </xsl:template>

</xsl:stylesheet>