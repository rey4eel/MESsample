<xsl:stylesheet version="1.0"
      xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      xmlns:ms="urn:schemas-microsoft-com:xslt"
      xmlns:dt="urn:schemas-microsoft-com:datatypes"
	    xmlns:cs="urn:scripts-csharp"
      xmlns:extobj ="urn:extension-object">
  <!-- script -->
  <ms:script language="C#" implements-prefix="cs">
  <![CDATA[

	public string GetDateTimeNow(string sFormat)
	{
		return DateTime.Now.ToString(sFormat);
	}

]]>
  </ms:script>
  
  <!-- output -->
  <!--<xsl:output method="text" version="1.0"/>-->
   <xsl:output method="xml" version="1.0" encoding="utf-8" indent="yes" />
  
  <!-- variables   -->

  <!-- params -->
  <!--
  <xsl:param name="AOICustomerCodes"/>
  <xsl:param name="SPICustomerCodes"/>
  <xsl:param name="DestPath"/>
  <xsl:param name="FileName"/>
  <xsl:param name="Where"/>
-->

<!-- Property -->
<xsl:param name="eventId"/>
<xsl:param name="statNoReview"/>
<xsl:param name="LaneReview"/>
<xsl:param name="SideReview"/>
<xsl:param name="BarcodeReview"/>
<xsl:param name="TypeNoReview"/>
<xsl:param name="TypeVarReview"/>
<xsl:param name="processNoReview"/>
<xsl:param name="processNameReview"/>


<!--Machine -->
<xsl:param name="lineNo"/>
<xsl:param name="statNo"/>

<xsl:param name="FrontFuNo"/>
<xsl:param name="FrontStatIdx"/>
<xsl:param name="FrontTopToolPos"/>
<xsl:param name="FrontBottomToolPos"/>
<xsl:param name="FrontTopWorkPos"/>
<xsl:param name="FrontBottomWorkPos"/>
<xsl:param name="FrontProcessName"/>
<xsl:param name="FrontApplication"/>
<xsl:param name="FrontTypeNo"/>
<xsl:param name="FrontTypeVar"/>
<xsl:param name="FrontTopProcessNo"/>
<xsl:param name="FrontBottomProcessNo"/>

<xsl:param name="RearFuNo"/>
<xsl:param name="RearStatIdx"/>
<xsl:param name="RearTopToolPos"/>
<xsl:param name="RearBottomToolPos"/>
<xsl:param name="RearTopWorkPos"/>
<xsl:param name="RearBottomWorkPos"/>
<xsl:param name="RearProcessName"/>
<xsl:param name="RearApplication"/>
<xsl:param name="RearTypeNo"/>
<xsl:param name="RearTypeVar"/>
<xsl:param name="RearTopProcessNo"/>
<xsl:param name="RearBottomProcessNo"/>

<xsl:template match="/">
	<xsl:element name="root"> <!-- root - start -->
		<xsl:element name="header"> <!-- header - start -->
			<xsl:attribute name="eventId"><xsl:value-of select="$eventId"/></xsl:attribute>
			<xsl:attribute name="eventName">partReceived</xsl:attribute>
			<xsl:attribute name="version">2.0</xsl:attribute>
			<xsl:attribute name="contentType">3</xsl:attribute>
			<xsl:attribute name="eventSwitch">-1</xsl:attribute>
			
			<xsl:element name="location"> <!-- location - start -->
				<xsl:attribute name="lineNo"><xsl:value-of select="$lineNo"/></xsl:attribute>
				<xsl:attribute name="statNo"><xsl:value-of select="$statNoReview"/></xsl:attribute>	
				
				<xsl:choose>
		                <xsl:when test="$LaneReview='0'"> <!-- Front -->
					<xsl:attribute name="fuNo"><xsl:value-of select="$FrontFuNo"/></xsl:attribute>		                
					<xsl:attribute name="statIdx"><xsl:value-of select="$FrontStatIdx"/></xsl:attribute>
					<xsl:choose>
						<xsl:when test="$SideReview='T'">
							<xsl:attribute name="workPos"><xsl:value-of select="$FrontTopWorkPos"/></xsl:attribute>
							<xsl:attribute name="toolPos"><xsl:value-of select="$FrontTopToolPos"/></xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="workPos"><xsl:value-of select="$FrontBottomWorkPos"/></xsl:attribute>
							<xsl:attribute name="toolPos"><xsl:value-of select="$FrontBottomToolPos"/></xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>					
					<xsl:attribute name="processNo"><xsl:value-of select="$processNoReview"/></xsl:attribute>
					<xsl:attribute name="processName"><xsl:value-of select="$processNameReview"/></xsl:attribute>
					<xsl:attribute name="application"><xsl:value-of select="$FrontApplication"/></xsl:attribute>		                
		                </xsl:when>
		                
		                <xsl:otherwise>   <!-- Rear -->
					<xsl:attribute name="fuNo"><xsl:value-of select="$RearFuNo"/></xsl:attribute>		                
					<xsl:attribute name="statIdx"><xsl:value-of select="$RearStatIdx"/></xsl:attribute>
					
					<xsl:choose>
						<xsl:when test="$SideReview='T'">
							<xsl:attribute name="workPos"><xsl:value-of select="$RearTopWorkPos"/></xsl:attribute>
							<xsl:attribute name="toolPos"><xsl:value-of select="$RearTopToolPos"/></xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="workPos"><xsl:value-of select="$RearBottomWorkPos"/></xsl:attribute>
							<xsl:attribute name="toolPos"><xsl:value-of select="$RearBottomToolPos"/></xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:attribute name="processNo"><xsl:value-of select="$processNoReview"/></xsl:attribute>
					<xsl:attribute name="processName"><xsl:value-of select="$processNameReview"/></xsl:attribute>
					<xsl:attribute name="application"><xsl:value-of select="$RearApplication"/></xsl:attribute>				                
		                </xsl:otherwise>		
				</xsl:choose>
				
			</xsl:element> <!-- location - end -->
		</xsl:element> <!-- header - end -->
		<xsl:element name="event"> <!-- event - start -->
			<xsl:element name="partReceived"> <!-- partReceived - start -->
				<xsl:attribute name="identifier"><xsl:value-of select="$BarcodeReview"/></xsl:attribute>
				<xsl:choose>
					<xsl:when test="$TypeNoReview!=''">
						<xsl:attribute name="typeNo"><xsl:value-of select="$TypeNoReview"/></xsl:attribute>
						<xsl:attribute name="typeVar"><xsl:value-of select="$TypeVarReview"/></xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:choose>
				                <xsl:when test="$LaneReview='0'"> <!-- Front -->
				                	<xsl:attribute name="typeNo"><xsl:value-of select="$FrontTypeNo"/></xsl:attribute>
							<xsl:attribute name="typeVar"><xsl:value-of select="$FrontTypeVar"/></xsl:attribute>
				                </xsl:when>
				                <xsl:otherwise>   <!-- Rear -->
				                	<xsl:attribute name="typeNo"><xsl:value-of select="$RearTypeNo"/></xsl:attribute>
							<xsl:attribute name="typeVar"><xsl:value-of select="$RearTypeVar"/></xsl:attribute>
				                </xsl:otherwise>		
						</xsl:choose>		                
					</xsl:otherwise>
				</xsl:choose>
			</xsl:element> <!-- partReceived - end -->
		</xsl:element> <!-- event - end -->	
		<xsl:element name="body"> <!-- body - start -->
		</xsl:element> <!-- body - end -->	
	</xsl:element> <!-- root - end -->
</xsl:template>

</xsl:stylesheet>

