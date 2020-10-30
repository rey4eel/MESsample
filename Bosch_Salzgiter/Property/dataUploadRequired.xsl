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
	public string BoolToString(string sBool)
	{
		string sRtn = "false";
		if (sBool == "1")
			sRtn = "true";
			
		return sRtn;
	}	
  ]]>
  </ms:script>
  
    <!-- output -->
  <!--<xsl:output method="text" version="1.0"/>-->
   <xsl:output method="xml" version="1.0" encoding="utf-8" indent="yes" />
   
<!-- Property -->
<xsl:param name="Lane"/>
<xsl:param name="Side"/>
<xsl:param name="eventId"/>
<xsl:param name="ProcessNo"/>

<!--Machine -->
<xsl:param name="lineNo"/>
<xsl:param name="statNo"/>
<xsl:param name="shift"/>
<xsl:param name="charge"/>
<xsl:param name="specPrgNo"/>
<xsl:param name="operationMode"/>
<xsl:param name="modeOn"/>
<xsl:param name="chainNo"/>

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

<xsl:param name="errorNo"/>
<xsl:param name="errorText"/>
<xsl:param name="batchStatus"/>
<xsl:param name="changeFlagStatus"/>
<xsl:param name="jamStatus"/>
<xsl:param name="partMissingStatus"/>

  <xsl:template match="/">
	<xsl:element name="root"> <!-- root - start -->
		<xsl:element name="header"> <!-- header - start -->
			<xsl:attribute name="eventId"><xsl:value-of select="$eventId"/></xsl:attribute>
			<xsl:attribute name="version">2.3</xsl:attribute>
			<xsl:attribute name="eventName">dataUploadRequired</xsl:attribute>
			<xsl:attribute name="eventSwitch">10</xsl:attribute>
			<xsl:attribute name="contentType">3</xsl:attribute>
			<!--xsl:attribute name="timeStamp">	
			<xsl:value-of select="extobj:DateTime_Now('yyyy-MM-ddTHH:mm:ss.fffffffK')"/>
			</xsl:attribute-->
			
			<xsl:element name="location"> <!-- location - start -->
				<xsl:attribute name="lineNo"><xsl:value-of select="$lineNo"/></xsl:attribute>
				<xsl:attribute name="statNo"><xsl:value-of select="$statNo"/></xsl:attribute>	
				
				<xsl:choose>
		                <xsl:when test="$Lane='0'"> <!-- Front -->
					<xsl:attribute name="fuNo"><xsl:value-of select="$FrontFuNo"/></xsl:attribute>		                
					<xsl:attribute name="statIdx"><xsl:value-of select="$FrontStatIdx"/></xsl:attribute>
					<xsl:choose>
						<xsl:when test="$Side='T'">
							<!--xsl:attribute name="workPos"><xsl:value-of select="$FrontTopWorkPos"/></xsl:attribute>
							<xsl:attribute name="toolPos"><xsl:value-of select="$FrontTopToolPos"/></xsl:attribute-->
							<xsl:attribute name="processNo"><xsl:value-of select="$FrontTopProcessNo"/></xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<!--xsl:attribute name="workPos"><xsl:value-of select="$FrontBottomWorkPos"/></xsl:attribute>
							<xsl:attribute name="toolPos"><xsl:value-of select="$FrontBottomToolPos"/></xsl:attribute-->
							<xsl:attribute name="processNo"><xsl:value-of select="$FrontBottomProcessNo"/></xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>					
					<xsl:attribute name="processName"><xsl:value-of select="$FrontProcessName"/></xsl:attribute>
					<xsl:attribute name="application"><xsl:value-of select="$FrontApplication"/></xsl:attribute>		                
		                </xsl:when>
		                
		                <xsl:otherwise>   <!-- Rear -->
					<xsl:attribute name="fuNo"><xsl:value-of select="$RearFuNo"/></xsl:attribute>		                
					<xsl:attribute name="statIdx"><xsl:value-of select="$RearStatIdx"/></xsl:attribute>
					
					<xsl:choose>
						<xsl:when test="$Side='T'">
							<!--xsl:attribute name="workPos"><xsl:value-of select="$RearTopWorkPos"/></xsl:attribute>
							<xsl:attribute name="toolPos"><xsl:value-of select="$RearTopToolPos"/></xsl:attribute-->
							<xsl:attribute name="processNo"><xsl:value-of select="$RearTopProcessNo"/></xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<!--xsl:attribute name="workPos"><xsl:value-of select="$RearBottomWorkPos"/></xsl:attribute>
							<xsl:attribute name="toolPos"><xsl:value-of select="$RearBottomToolPos"/></xsl:attribute-->
							<xsl:attribute name="processNo"><xsl:value-of select="$RearBottomProcessNo"/></xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:attribute name="processName"><xsl:value-of select="$RearProcessName"/></xsl:attribute>
					<xsl:attribute name="application"><xsl:value-of select="$RearApplication"/></xsl:attribute>				                
		                </xsl:otherwise>		
				</xsl:choose>
				
			</xsl:element> <!-- location - end -->
		</xsl:element> <!-- header - end -->
		<xsl:element name="event"> <!-- event - start -->
			<xsl:element name="dataUploadRequired"> <!-- dataUploadRequired start -->
			</xsl:element> <!-- dataUploadRequired end -->
		</xsl:element> <!-- event - end -->	
		<xsl:element name="body"> <!-- body - start -->
			<xsl:element name="items"> <!-- items start -->
				<xsl:element name="item">
					<xsl:attribute name="name">modeOn</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="cs:BoolToString($modeOn)"/></xsl:attribute>
					<xsl:attribute name="dataType">11</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">operationMode</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$operationMode"/></xsl:attribute>
					<xsl:attribute name="dataType">2</xsl:attribute>
				</xsl:element>	
				<xsl:element name="item">
					<xsl:attribute name="name">typeNo</xsl:attribute>
					<xsl:attribute name="value">
						<xsl:choose>
				                <xsl:when test="$Lane='0'"> <!-- Front -->
				                	<xsl:value-of select="$FrontTypeNo"/>
				                </xsl:when>
				                <xsl:otherwise>   <!-- Rear -->				                	
				                	<xsl:value-of select="$RearTypeNo"/>
				                </xsl:otherwise>		
						</xsl:choose>    				                	
					</xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>	
				<xsl:element name="item">
					<xsl:attribute name="name">typeVar</xsl:attribute>
					<xsl:attribute name="value">
						<xsl:choose>
				                <xsl:when test="$Lane='0'"> <!-- Front -->
				                	<xsl:value-of select="$FrontTypeVar"/>
				                </xsl:when>
				                <xsl:otherwise>   <!-- Rear -->				                	
				                	<xsl:value-of select="$RearTypeVar"/>
				                </xsl:otherwise>		
						</xsl:choose>    				                	
					</xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">batch</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$batchStatus"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">changeFlag</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$changeFlagStatus"/></xsl:attribute>
					<xsl:attribute name="dataType">11</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">jam</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$jamStatus"/></xsl:attribute>
					<xsl:attribute name="dataType">11</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">partMissing</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$partMissingStatus"/></xsl:attribute>
					<xsl:attribute name="dataType">11</xsl:attribute>
				</xsl:element>	
				<xsl:element name="item">
					<xsl:attribute name="name">errorNo</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$errorNo"/></xsl:attribute>
					<xsl:attribute name="dataType">3</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">errorText</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$errorText"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>	
			</xsl:element> <!-- items end -->
		</xsl:element> <!-- body - end -->	
	</xsl:element> <!-- root - end -->	
</xsl:template>

</xsl:stylesheet>