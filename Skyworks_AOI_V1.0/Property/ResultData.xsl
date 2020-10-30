<xsl:stylesheet version="1.0"
      xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
      xmlns:ms="urn:schemas-microsoft-com:xslt"
      xmlns:dt="urn:schemas-microsoft-com:datatypes"
	    xmlns:cs="urn:scripts-csharp"
      xmlns:extobj ="urn:extension-object">
  <!-- output -->
<xsl:output method="text" version="1.0"/>
  <!-- variables   -->
<xsl:variable name="newline" select="'&#xA;'" />
<xsl:variable name="delimiter" select="','"/>
  <!-- params -->

<xsl:template match="/">
  
  <!-- Insert here -->
	<xsl:value-of select="InspectResult/PCB/@ArrayCnt"/>
	<xsl:value-of select="$newline"/>
	<xsl:value-of select="InspectResult/PCB/@ArrayCnt*InspectResult/PCB/@PCBTotalComp"/>
	<xsl:value-of select="$newline"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@ResultAfterName='NG'])"/>
	<xsl:value-of select="$newline"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@ResultAfterName='PASS'])"/>
	<xsl:value-of select="$newline"/>
	
	
	<xsl:value-of select="InspectResult/PCB/@UserID"/>
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="InspectResult/PCB/@PCBID"/>
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="InspectResult/PCB/@BarCode"/>
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="InspectResult/PCB/@Lot"/>
	<xsl:value-of select="$delimiter"/>
	<xsl:if test="InspectResult/PCB/@PCBResultBeforeName='GOOD'">
		<xsl:text>0</xsl:text>
	</xsl:if>
	<xsl:if test="InspectResult/PCB/@PCBResultAfterName='NG'">
		<xsl:text>2</xsl:text>
	</xsl:if>
	<xsl:if test="InspectResult/PCB/@PCBResultAfterName='PASS'">
		<xsl:text>1</xsl:text>
	</xsl:if>
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000000' and @ResultAfterName='NG'])"/><!--Defect1:UNDEFINED-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000001' and @ResultAfterName='NG'])"/><!--Defect2:PADOVERHANG-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000002' and @ResultAfterName='NG'])"/><!--Defect3:COPLANARITY-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000003' and @ResultAfterName='NG'])"/><!--Defect4:DIMENSION-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000004' and @ResultAfterName='NG'])"/><!--Defect5:MISSING-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000005' and @ResultAfterName='NG'])"/><!--Defect6:COMPONENT_SHIFT-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000006' and @ResultAfterName='NG'])"/><!--Defect7:UPSIDEDOWN-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000007' and @ResultAfterName='NG'])"/><!--Defect8:POSITION-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000008' and @ResultAfterName='NG'])"/><!--Defect9:SOLDER_JOINT-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000009' and @ResultAfterName='NG'])"/><!--Defect10:LIFTED_LEAD-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000010' and @ResultAfterName='NG'])"/><!--Defect11:LIFTED_BODY-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000011' and @ResultAfterName='NG'])"/><!--Defect12:BRIDGING-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000012' and @ResultAfterName='NG'])"/><!--Defect13:BILL_BOARDING-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000013' and @ResultAfterName='NG'])"/><!--Defect14:TOMBSTONE-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000014' and @ResultAfterName='NG'])"/><!--Defect15:BODY_DIMENSION-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000015' and @ResultAfterName='NG'])"/><!--Defect16:POLARITY-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000016' and @ResultAfterName='NG'])"/><!--Defect17:OCR_OCV-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000017' and @ResultAfterName='NG'])"/><!--Defect18:ABSENCE-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000018' and @ResultAfterName='NG'])"/><!--Defect19:OVERHANG-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000019' and @ResultAfterName='NG'])"/><!--Defect20:MISSING_LEAD-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000020' and @ResultAfterName='NG'])"/><!--Defect21:PARTICLE-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000021' and @ResultAfterName='NG'])"/><!--Defect22:FOREIGNMATERIAL_BODY-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorDefect='30000022' and @ResultAfterName='NG'])"/><!--Defect23:FOREIGNMATERIAL_LEAD-->
	<xsl:value-of select="$delimiter"/>
	<xsl:text>0</xsl:text><!--Defect24:NONE-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@ResultAfterName='PASS'])"/><!--Defect1:UNDEFINED : 30000000-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000001' and @ResultAfterName='PASS'])"/><!--Defect2:PADOVERHANG-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000002' and @ResultAfterName='PASS'])"/><!--Defect3:COPLANARITY-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000003' and @ResultAfterName='PASS'])"/><!--Defect4:DIMENSION-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000004' and @ResultAfterName='PASS'])"/><!--Defect5:MISSING-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000005' and @ResultAfterName='PASS'])"/><!--Defect6:COMPONENT_SHIFT-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000006' and @ResultAfterName='PASS'])"/><!--Defect7:UPSIDEDOWN-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000007' and @ResultAfterName='PASS'])"/><!--Defect8:POSITION-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000008' and @ResultAfterName='PASS'])"/><!--Defect9:SOLDER_JOINT-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000009' and @ResultAfterName='PASS'])"/><!--Defect10:LIFTED_LEAD-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000010' and @ResultAfterName='PASS'])"/><!--Defect11:LIFTED_BODY-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000011' and @ResultAfterName='PASS'])"/><!--Defect12:BRIDGING-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000012' and @ResultAfterName='PASS'])"/><!--Defect13:BILL_BOARDING-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000013' and @ResultAfterName='PASS'])"/><!--Defect14:TOMBSTONE-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000014' and @ResultAfterName='PASS'])"/><!--Defect15:BODY_DIMENSION-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000015' and @ResultAfterName='PASS'])"/><!--Defect16:POLARITY-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000016' and @ResultAfterName='PASS'])"/><!--Defect17:OCR_OCV-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000017' and @ResultAfterName='PASS'])"/><!--Defect18:ABSENCE-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000018' and @ResultAfterName='PASS'])"/><!--Defect19:OVERHANG-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000019' and @ResultAfterName='PASS'])"/><!--Defect20:MISSING_LEAD-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000020' and @ResultAfterName='PASS'])"/><!--Defect21:PARTICLE-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000021' and @ResultAfterName='PASS'])"/><!--Defect22:FOREIGNMATERIAL_BODY-->
	<xsl:value-of select="$delimiter"/>
	<xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/Component[@MajorFailure='30000022' and @ResultAfterName='PASS'])"/><!--Defect23:FOREIGNMATERIAL_LEAD-->
	<xsl:value-of select="$delimiter"/>
	<xsl:text>0</xsl:text><!--Defect24:NONE-->
	<xsl:value-of select="$newline"/>	
	
	<xsl:for-each select="InspectResult/PCB/PanelGroup/Panel">
		<xsl:value-of select="@PanelIndex"/>
		<xsl:value-of select="$delimiter"/>
		<xsl:if test="@PanelResultAfterName='NG'">
		<xsl:text>2</xsl:text>
		</xsl:if>
		<xsl:if test="@PanelResultAfterName='PASS'">
		<xsl:text>1</xsl:text>
		</xsl:if>
		<xsl:if test="@PanelResultAfterName='GOOD'">
		<xsl:text>0</xsl:text>
		</xsl:if>
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="../../@PCBTotalComp"/>
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@ResultAfterName='NG'])"/>
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@ResultAfterName='PASS'])"/>
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000000' and @ResultAfterName='NG'])"/><!--Defect1:UNDEFINED-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000001' and @ResultAfterName='NG'])"/><!--Defect2:PADOVERHANG-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000002' and @ResultAfterName='NG'])"/><!--Defect3:COPLANARITY-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000003' and @ResultAfterName='NG'])"/><!--Defect4:DIMENSION-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000004' and @ResultAfterName='NG'])"/><!--Defect5:MISSING-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000005' and @ResultAfterName='NG'])"/><!--Defect6:COMPONENT_SHIFT-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000006' and @ResultAfterName='NG'])"/><!--Defect7:UPSIDEDOWN-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000007' and @ResultAfterName='NG'])"/><!--Defect8:POSITION-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000008' and @ResultAfterName='NG'])"/><!--Defect9:SOLDER_JOINT-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000009' and @ResultAfterName='NG'])"/><!--Defect10:LIFTED_LEAD-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000010' and @ResultAfterName='NG'])"/><!--Defect11:LIFTED_BODY-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000011' and @ResultAfterName='NG'])"/><!--Defect12:BRIDGING-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000012' and @ResultAfterName='NG'])"/><!--Defect13:BILL_BOARDING-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000013' and @ResultAfterName='NG'])"/><!--Defect14:TOMBSTONE-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000014' and @ResultAfterName='NG'])"/><!--Defect15:BODY_DIMENSION-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000015' and @ResultAfterName='NG'])"/><!--Defect16:POLARITY-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000016' and @ResultAfterName='NG'])"/><!--Defect17:OCR_OCV-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000017' and @ResultAfterName='NG'])"/><!--Defect18:ABSENCE-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000018' and @ResultAfterName='NG'])"/><!--Defect19:OVERHANG-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000019' and @ResultAfterName='NG'])"/><!--Defect20:MISSING_LEAD-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000020' and @ResultAfterName='NG'])"/><!--Defect21:PARTICLE-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000021' and @ResultAfterName='NG'])"/><!--Defect22:FOREIGNMATERIAL_BODY-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorDefect='30000022' and @ResultAfterName='NG'])"/><!--Defect23:FOREIGNMATERIAL_LEAD-->
		<xsl:value-of select="$delimiter"/>
		<xsl:text>0</xsl:text><!--Defect24:NONE-->
		<xsl:value-of select="$delimiter"/>
		
		<xsl:value-of select="count(Component[@ResultAfterName='PASS'])"/><!--Defect1:UNDEFINED : 30000000 -->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000001' and @ResultAfterName='PASS'])"/><!--Defect2:PADOVERHANG-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000002' and @ResultAfterName='PASS'])"/><!--Defect3:COPLANARITY-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000003' and @ResultAfterName='PASS'])"/><!--Defect4:DIMENSION-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000004' and @ResultAfterName='PASS'])"/><!--Defect5:MISSING-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000005' and @ResultAfterName='PASS'])"/><!--Defect6:COMPONENT_SHIFT-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000006' and @ResultAfterName='PASS'])"/><!--Defect7:UPSIDEDOWN-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000007' and @ResultAfterName='PASS'])"/><!--Defect8:POSITION-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000008' and @ResultAfterName='PASS'])"/><!--Defect9:SOLDER_JOINT-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000009' and @ResultAfterName='PASS'])"/><!--Defect10:LIFTED_LEAD-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000010' and @ResultAfterName='PASS'])"/><!--Defect11:LIFTED_BODY-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000011' and @ResultAfterName='PASS'])"/><!--Defect12:BRIDGING-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000012' and @ResultAfterName='PASS'])"/><!--Defect13:BILL_BOARDING-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000013' and @ResultAfterName='PASS'])"/><!--Defect14:TOMBSTONE-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000014' and @ResultAfterName='PASS'])"/><!--Defect15:BODY_DIMENSION-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000015' and @ResultAfterName='PASS'])"/><!--Defect16:POLARITY-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000016' and @ResultAfterName='PASS'])"/><!--Defect17:OCR_OCV-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000017' and @ResultAfterName='PASS'])"/><!--Defect18:ABSENCE-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000018' and @ResultAfterName='PASS'])"/><!--Defect19:OVERHANG-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000019' and @ResultAfterName='PASS'])"/><!--Defect20:MISSING_LEAD-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000020' and @ResultAfterName='PASS'])"/><!--Defect21:PARTICLE-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000021' and @ResultAfterName='PASS'])"/><!--Defect22:FOREIGNMATERIAL_BODY-->
		<xsl:value-of select="$delimiter"/>
		<xsl:value-of select="count(Component[@MajorFailure='30000022' and @ResultAfterName='PASS'])"/><!--Defect23:FOREIGNMATERIAL_LEAD-->
		<xsl:value-of select="$delimiter"/>
		<xsl:text>0</xsl:text><!--Defect24:NONE-->
		<xsl:value-of select="$newline"/>
	</xsl:for-each>

  
</xsl:template>

</xsl:stylesheet>