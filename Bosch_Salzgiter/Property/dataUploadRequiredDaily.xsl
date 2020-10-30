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

<xsl:param name="machineDescription"/>
<xsl:param name="machineSupplier"/>
<xsl:param name="machineType"/>
<xsl:param name="machineProductionYear"/>
<xsl:param name="serialNumber"/>
<xsl:param name="machineSystemSoftware"/>
<xsl:param name="softwareServicePack"/>
<xsl:param name="plant"/>
<xsl:param name="machineDokuNo"/>
<xsl:param name="machineSoftwareVersion"/>
<xsl:param name="interfaceSoftwareVersionNo"/>
<xsl:param name="machineNumberOfProductionArea"/>


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
			<xsl:attribute name="eventSwitch">105</xsl:attribute>
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
							<xsl:attribute name="processNo"><xsl:value-of select="$FrontTopProcessNo"/></xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
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
							<xsl:attribute name="processNo"><xsl:value-of select="$RearTopProcessNo"/></xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
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
					<xsl:attribute name="name">machineDescription</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$machineDescription"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">machineSupplier</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$machineSupplier"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>	
				<xsl:element name="item">
					<xsl:attribute name="name">machineType</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$machineType"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">machineProductionYear</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$machineProductionYear"/></xsl:attribute>
					<xsl:attribute name="dataType">3</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">serialNumber</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$serialNumber"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">machineSystemSoftware</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$machineSystemSoftware"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>	
				<xsl:element name="item">
					<xsl:attribute name="name">softwareServicePack</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$softwareServicePack"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">plant</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$plant"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>	
				<xsl:element name="item">
					<xsl:attribute name="name">machineDokuNo</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$machineDokuNo"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>	
				<xsl:element name="item">
					<xsl:attribute name="name">machineSoftwareVersion</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$machineSoftwareVersion"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>	
				<xsl:element name="item">
					<xsl:attribute name="name">interfaceSoftwareVersionNo</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$interfaceSoftwareVersionNo"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>	
				<xsl:element name="item">
					<xsl:attribute name="name">xmlVersion</xsl:attribute>
					<xsl:attribute name="value"><xsl:text>XML2.3</xsl:text></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>	
				<xsl:element name="item">
					<xsl:attribute name="name">machineTime</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="extobj:DateTime_Now('yyyy-MM-ddTHH:mm:ss.fffffffK')"/></xsl:attribute>
					<xsl:attribute name="dataType">8</xsl:attribute>
				</xsl:element>	
				<xsl:element name="item">
					<xsl:attribute name="name">machineNumberOfProductionArea</xsl:attribute>
					<xsl:attribute name="value"><xsl:value-of select="$machineNumberOfProductionArea"/></xsl:attribute>
					<xsl:attribute name="dataType">3</xsl:attribute>
				</xsl:element>	
			</xsl:element> <!-- items end -->
		</xsl:element> <!-- body - end -->	
	</xsl:element> <!-- root - end -->	
</xsl:template>

</xsl:stylesheet>