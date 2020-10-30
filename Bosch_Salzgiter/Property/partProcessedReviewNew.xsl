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
<xsl:param name="workingCodeReview"/>

<!--Machine -->
<xsl:param name="lineNo"/>
<xsl:param name="statNo"/>
<xsl:param name="CallFrom"/>

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

<xsl:param name="workingCode"/>
<xsl:param name="NIO"/>
<xsl:param name="machineID"/>

<!--Judge-->
<xsl:param name="ResultGood"/>
<xsl:param name="ResultBad"/>
<xsl:param name="PartOK"/>
<xsl:param name="PartNOK"/>

<xsl:param name="ArrayBadmark"/>
<xsl:param name="nioBitsArrayGood"/>
<xsl:param name="nioBitsArrayNG"/>
<xsl:param name="nioBitsAllArraysBadmark"/>
<xsl:param name="nioBitsBoardGood"/>
<xsl:param name="nioBitsBoardNG"/>

  <xsl:template match="/">
	<xsl:element name="root"> <!-- root - start -->
		<xsl:element name="header"> <!-- header - start -->
			<xsl:attribute name="eventId"><xsl:value-of select="$eventId"/></xsl:attribute>
			<xsl:attribute name="version">2.0</xsl:attribute>
			<xsl:attribute name="eventName">partProcessed</xsl:attribute>
			<xsl:attribute name="eventSwitch">-1</xsl:attribute>
			<xsl:attribute name="contentType">3</xsl:attribute>
			


			<xsl:element name="location"> <!-- location - start -->
				<xsl:attribute name="lineNo"><xsl:value-of select="$lineNo"/></xsl:attribute>
				<xsl:attribute name="statNo"><xsl:value-of select="$statNoReview"/></xsl:attribute>	
				
				<xsl:choose>
		                <xsl:when test="InspectResult/PCB/@Lane='0'"> <!-- Front -->
					<xsl:attribute name="fuNo"><xsl:value-of select="$FrontFuNo"/></xsl:attribute>		                
					<xsl:attribute name="statIdx"><xsl:value-of select="$FrontStatIdx"/></xsl:attribute>
					<xsl:choose>
						<xsl:when test="InspectResult/PCB/@TB='T'">
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
						<xsl:when test="InspectResult/PCB/@TB='T'">
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
			<xsl:element name="partProcessed"> <!-- partProcessed - start -->
				<xsl:attribute name="identifier"><xsl:value-of select="InspectResult/PCB/@BarCode"/></xsl:attribute>
			</xsl:element> <!-- partProcessed - end -->
		</xsl:element> <!-- event - end -->


		<xsl:element name="body"> <!-- body - start -->
			<xsl:element name="structs">
				<xsl:element name="resHead">
		      			<xsl:attribute name="result">
		      				<xsl:choose>
							<xsl:when test="count(InspectResult/PCB/PanelGroup/Panel[@PanelBadMark='0']) = 0">
								<xsl:value-of select="$ArrayBadmark"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:choose>
						                <xsl:when test="InspectResult/PCB/@PCBResultAfterName='NG'">
						                	<xsl:value-of select="$ResultBad"/>
						                </xsl:when>
						                <xsl:otherwise>
						                	<xsl:value-of select="$ResultGood"/>
						                </xsl:otherwise>
								</xsl:choose>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:attribute>
					
					
					<xsl:attribute name="typeNo"><xsl:value-of select="$TypeNoReview"/></xsl:attribute>
					<xsl:attribute name="typeVar"><xsl:value-of select="$TypeVarReview"/></xsl:attribute>
					<!--xsl:choose>
			                <xsl:when test="InspectResult/PCB/@TB='T'">
			                	<xsl:attribute name="typeNo"><xsl:value-of select="$FrontTypeNo"/></xsl:attribute>
			                	<xsl:attribute name="typeVar"><xsl:value-of select="$FrontTypeVar"/></xsl:attribute>
			                </xsl:when>
			                <xsl:otherwise>
			                	<xsl:attribute name="typeNo"><xsl:value-of select="$RearTypeNo"/></xsl:attribute>
			                	<xsl:attribute name="typeVar"><xsl:value-of select="$RearTypeVar"/></xsl:attribute>
			                </xsl:otherwise>		
					</xsl:choose-->	
					
			             <xsl:attribute name="workingCode"><xsl:value-of select="$workingCodeReview"/></xsl:attribute>
			             
		      			<xsl:attribute name="nioBits">
		      				<xsl:text>0</xsl:text>
		      			</xsl:attribute>
		      			
		      			<xsl:attribute name="machineID"><xsl:value-of select="$machineID"/></xsl:attribute>

					<xsl:attribute name="targetIdx">
						<xsl:if test="count(InspectResult/PCB/PanelGroup/Panel[@PanelBadMark='0']) = 0">
							<xsl:text>3</xsl:text>
						</xsl:if>	
						<xsl:if test="count(InspectResult/PCB/PanelGroup/Panel[@PanelBadMark='0']) > 0">
						   <xsl:choose>
					                <xsl:when test="InspectResult/PCB/@PCBResultAfterName='NG'">
					                	<xsl:text>2</xsl:text>
					                </xsl:when>
					                <xsl:otherwise>
					                	<xsl:text>1</xsl:text>
					                </xsl:otherwise>
				                </xsl:choose>
						</xsl:if>
					</xsl:attribute>						
					<xsl:attribute name="partState">
						<xsl:if test="count(InspectResult/PCB/PanelGroup/Panel[@PanelBadMark='0']) = 0">
							<xsl:value-of select="$ArrayBadmark"/>
						</xsl:if>	
						<xsl:if test="count(InspectResult/PCB/PanelGroup/Panel[@PanelBadMark='0']) > 0">
						   <xsl:choose>
					                <xsl:when test="InspectResult/PCB/@PCBResultAfterName='NG'">
					                	<xsl:value-of select="$PartNOK"/>
					                </xsl:when>
					                <xsl:otherwise>
					                	<xsl:value-of select="$PartOK"/>
					                </xsl:otherwise>
				                </xsl:choose>
						</xsl:if>
					</xsl:attribute>

				</xsl:element>				
			</xsl:element> 
			
			<xsl:element name="items">
				<xsl:element name="item">
					<xsl:attribute name="name">apiProg1Name</xsl:attribute> 
					<xsl:attribute name="value"><xsl:value-of select="InspectResult/PCB/@JobFileName"/></xsl:attribute> 
					<xsl:attribute name="dataType">3</xsl:attribute>
				</xsl:element>
				<xsl:element name="item">
					<xsl:attribute name="name">WorkPart.FailCount</xsl:attribute> 
					<xsl:attribute name="value"><xsl:value-of select="count(InspectResult/PCB/PanelGroup/Panel/@PanelIndex)"/></xsl:attribute> 
					<xsl:attribute name="dataType">3</xsl:attribute>
				</xsl:element>
			</xsl:element> 
			
			<xsl:element name="structArrays">
			
				<xsl:if test="count(InspectResult/PCB/PanelGroup/Panel) > 1">
					<xsl:element name="array">
						<xsl:attribute name="name">results</xsl:attribute>
						<xsl:element name="structDef">
							<xsl:element name="item">
								<xsl:attribute name="name">pos</xsl:attribute>
								<xsl:attribute name="dataType">3</xsl:attribute>
							</xsl:element>
							<xsl:element name="item">
								<xsl:attribute name="name">result</xsl:attribute>
								<xsl:attribute name="dataType">3</xsl:attribute>
							</xsl:element>
							<xsl:element name="item">
								<xsl:attribute name="name">targetIdx</xsl:attribute>
								<xsl:attribute name="dataType">3</xsl:attribute>
							</xsl:element>
							<xsl:element name="item">
								<xsl:attribute name="name">state</xsl:attribute>
								<xsl:attribute name="dataType">3</xsl:attribute>
							</xsl:element>						
							<xsl:element name="item">
								<xsl:attribute name="name">identifier</xsl:attribute>
								<xsl:attribute name="dataType">8</xsl:attribute>
							</xsl:element>						
						</xsl:element>		
						<xsl:element name="values">	
							<xsl:for-each select="InspectResult/PCB/PanelGroup/Panel/@PanelIndex">
								<xsl:element name="item">
									<xsl:attribute name="pos"><xsl:value-of select="../@PanelIndex"/></xsl:attribute>
									
									<xsl:attribute name="result">
										<xsl:choose>
											<xsl:when test="../@PanelBadMark='1'">
												<xsl:value-of select="$ArrayBadmark"/>
											</xsl:when>
											<xsl:otherwise>
												<xsl:choose>
										                <xsl:when test="../@PCBResultAfterName='NG'">
													<xsl:value-of select="$ResultBad"/>
										                </xsl:when>
										                <xsl:otherwise>
													<xsl:value-of select="$ResultGood"/>
										                </xsl:otherwise>
												</xsl:choose>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
									
									<xsl:attribute name="targetIdx"><!--targetIdx-->
										<xsl:choose>
											<xsl:when test="../@PanelBadMark='1'">
												<xsl:text>3</xsl:text>
											</xsl:when>
											<xsl:otherwise>
												<xsl:choose>
										                <xsl:when test="../@PCBResultAfterName='NG'">
													<xsl:text>2</xsl:text>
										                </xsl:when>
										                <xsl:otherwise>
													<xsl:text>1</xsl:text>
										                </xsl:otherwise>
												</xsl:choose>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute><!--targetIdx-->
									
									
									<xsl:attribute name="state"><!--state-->
										<xsl:choose>
											<xsl:when test="../@PanelBadMark='1'">
												<xsl:value-of select="$ArrayBadmark"/>											
											</xsl:when>
											<xsl:otherwise>
												<xsl:choose>
										                <xsl:when test="../@PCBResultAfterName='NG'">
													<xsl:value-of select="$PartNOK"/>
										                </xsl:when>
										                <xsl:otherwise>
													<xsl:value-of select="$PartOK"/>
										                </xsl:otherwise>
												</xsl:choose>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute><!--state-->
	
									<xsl:attribute name="identifier"><!--identifier-->
										<xsl:value-of select="../@PanelBarCode"/>
									</xsl:attribute><!--identifier-->
	
									
								</xsl:element>
							</xsl:for-each>
						</xsl:element>
					</xsl:element>
				</xsl:if>

				<xsl:element name="array">
					<xsl:attribute name="name">API</xsl:attribute>
					
					<xsl:element name="structDef">
						<xsl:element name="item">
							<xsl:attribute name="name">boardNo</xsl:attribute>
							<xsl:attribute name="dataType">3</xsl:attribute>
						</xsl:element>
						<xsl:element name="item">
							<xsl:attribute name="name">referenceDesignator</xsl:attribute>
							<xsl:attribute name="dataType">8</xsl:attribute>
						</xsl:element>
						<xsl:element name="item">
							<xsl:attribute name="name">componentPartNo</xsl:attribute>
							<xsl:attribute name="dataType">8</xsl:attribute>
						</xsl:element>
						<xsl:element name="item">
							<xsl:attribute name="name">connectorNo</xsl:attribute>
							<xsl:attribute name="dataType">8</xsl:attribute>
						</xsl:element>						
						<xsl:element name="item">
							<xsl:attribute name="name">componentPinNo</xsl:attribute>
							<xsl:attribute name="dataType">8</xsl:attribute>
						</xsl:element>						
						<xsl:element name="item">
							<xsl:attribute name="name">apiTestWindowNo</xsl:attribute>
							<xsl:attribute name="dataType">8</xsl:attribute>
						</xsl:element>						
						<xsl:element name="item">
							<xsl:attribute name="name">apiTestAlgorithm</xsl:attribute>
							<xsl:attribute name="dataType">8</xsl:attribute>
						</xsl:element>						
						<xsl:element name="item">
							<xsl:attribute name="name">apiFailedFeatureValue</xsl:attribute>
							<xsl:attribute name="dataType">3</xsl:attribute>
						</xsl:element>						
						<xsl:element name="item">
							<xsl:attribute name="name">apiComponentShape</xsl:attribute>
							<xsl:attribute name="dataType">8</xsl:attribute>
						</xsl:element>						
						<xsl:element name="item">
							<xsl:attribute name="name">apiFailureCode</xsl:attribute>
							<xsl:attribute name="dataType">3</xsl:attribute>
						</xsl:element>						
						<xsl:element name="item">
							<xsl:attribute name="name">apiFailureClassification</xsl:attribute>
							<xsl:attribute name="dataType">3</xsl:attribute>
						</xsl:element>						
						<xsl:element name="item">
							<xsl:attribute name="name">apiScrapConfirmation</xsl:attribute>
							<xsl:attribute name="dataType">3</xsl:attribute>
						</xsl:element>						
						<xsl:element name="item">
							<xsl:attribute name="name">checkType</xsl:attribute>
							<xsl:attribute name="dataType">8</xsl:attribute>
						</xsl:element>						
						<xsl:element name="item">
							<xsl:attribute name="name">checkValue</xsl:attribute>
							<xsl:attribute name="dataType">8</xsl:attribute>
						</xsl:element>
					</xsl:element>		
					
					<xsl:element name="values">	
						<xsl:for-each select="InspectResult/PCB/PanelGroup/Panel/Component/Defect[@FailureName!='GOOD' and (@InspTypeName='Dimension' or @InspTypeName='Missing')]">
						        <!--xsl:variable name="oX">
						        	<xsl:value-of select="../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=9]/@Value div 1000"/>
						        </xsl:variable>
						        <xsl:variable name="oY">
						        	<xsl:value-of select="../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=12]/@Value div 1000"/>
						        </xsl:variable>
						        <xsl:variable name="iX">
						        	<xsl:value-of select="(../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=11]/@Value - ((../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=11]/@Value - ../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=10]/@Value) div 2)) div 1000"/>
						        </xsl:variable>
						        <xsl:variable name="iY">
						        	<xsl:value-of select="(../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=14]/@Value - ((../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=14]/@Value - ../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=13]/@Value) div 2)) div 1000"/>
						        </xsl:variable-->
						        <!--xsl:variable name="oZ">
						        	<xsl:value-of select="format-number(../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=15]/@Value div 1000, '0')"/>
						        </xsl:variable>
						        <xsl:variable name="iZ">
						        	<xsl:value-of select="format-number((../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=17]/@Value - ((../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=17]/@Value - ../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=16]/@Value) div 2)) div 1000, '0')"/>
						        </xsl:variable-->

						        <xsl:variable name="oZ">
								<xsl:choose>
									<xsl:when test="@InspTypeName='Dimension'">
								        	<xsl:value-of select="format-number(../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=15]/@Value div 1000, '0')"/>
									</xsl:when>
									<xsl:when test="@InspTypeName='Missing'">
								        	<xsl:value-of select="format-number(../Defect[@InspTypeName='Missing']/FlexibleField[@Index=0]/@Value div 1000, '0')"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>0</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
						        </xsl:variable>
						        <xsl:variable name="iZ">
								<xsl:choose>
									<xsl:when test="@InspTypeName='Dimension'">
								        	<xsl:value-of select="format-number((../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=17]/@Value - ((../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=17]/@Value - ../Defect[@InspTypeName='Dimension']/FlexibleField[@Index=16]/@Value) div 2)) div 1000, '0')"/>
									</xsl:when>
									<xsl:when test="@InspTypeName='Missing'">
								        	<xsl:value-of select="format-number(../Defect[@InspTypeName='Missing']/FlexibleField[@Index=1]/@Value div 1000, '0.##')"/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:text>0</xsl:text>
									</xsl:otherwise>
								</xsl:choose>
						        </xsl:variable>
						        
							<xsl:element name="item">
								<xsl:attribute name="boardNo"><xsl:value-of select="position()"/></xsl:attribute>
								<xsl:attribute name="referenceDesignator"><xsl:value-of select="../@uname"/></xsl:attribute>
								<xsl:attribute name="componentPartNo"><xsl:value-of select="../@PartNumber"/></xsl:attribute>
								<xsl:attribute name="connectorNo"><xsl:value-of select="../@ArrayIndex"/></xsl:attribute>
								<xsl:attribute name="componentPinNo"><xsl:value-of select="@LeadName"/></xsl:attribute>
								<xsl:attribute name="apiTestWindowNo"><xsl:value-of select="@FOVNo"/></xsl:attribute>
								<xsl:attribute name="apiTestAlgorithm"><xsl:value-of select="@InspTypeCustomerName"/></xsl:attribute>
								
								<xsl:attribute name="apiFailedFeatureValue">
									<xsl:value-of select="ms:node-set($oZ)"/>
								</xsl:attribute>
								
								<xsl:attribute name="apiComponentShape">
									<!--xsl:value-of select="ms:node-set($iZ)"/-->
									<xsl:value-of select="../@PartNumber"/>
								</xsl:attribute>
								
								<xsl:attribute name="apiFailureCode"><xsl:value-of select="@InspTypeCustomerCode"/></xsl:attribute>
								<xsl:attribute name="apiFailureClassification">
									<xsl:choose>
										<xsl:when test="@DefectName='PASS'">
											<xsl:text>1</xsl:text>
										</xsl:when>
										<xsl:otherwise>
											<xsl:text>0</xsl:text>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<xsl:attribute name="apiScrapConfirmation">
									<xsl:choose>
										<xsl:when test="../../@PanelBadMark='1' or ../../@PanelResultBeforeName='SCRAP'">
											<xsl:text>1</xsl:text>
										</xsl:when>
										<xsl:otherwise>
											<xsl:text>0</xsl:text>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								
								<xsl:attribute name="checkType"><xsl:value-of select="@InspTypeCustomerName"/></xsl:attribute>
								<xsl:attribute name="checkValue"><xsl:value-of select="ms:node-set($oZ)-ms:node-set($iZ)"/></xsl:attribute>
							</xsl:element>
						</xsl:for-each>
					</xsl:element>
					
				</xsl:element>



				<xsl:element name="array">
					<xsl:attribute name="name">statisticPinData</xsl:attribute>
					
					<xsl:element name="structDef">
						<xsl:element name="item">
							<xsl:attribute name="name">pos</xsl:attribute>
							<xsl:attribute name="dataType">3</xsl:attribute>
						</xsl:element>
						<xsl:element name="item">
							<xsl:attribute name="name">countPin</xsl:attribute>
							<xsl:attribute name="dataType">3</xsl:attribute>
						</xsl:element>
						<xsl:element name="item">
							<xsl:attribute name="name">errorCountPin</xsl:attribute>
							<xsl:attribute name="dataType">3</xsl:attribute>
						</xsl:element>
						<xsl:element name="item">
							<xsl:attribute name="name">passCountPin</xsl:attribute>
							<xsl:attribute name="dataType">3</xsl:attribute>
						</xsl:element>						
					</xsl:element>		
					
					<xsl:element name="values">	
						<xsl:for-each select="InspectResult/PCB/PanelGroup/Panel">
							<xsl:element name="item">
								<xsl:attribute name="pos"><xsl:value-of select="@PanelIndex"/></xsl:attribute>
								<xsl:attribute name="countPin"><xsl:value-of select="count(Component/Defect)"/></xsl:attribute>
								<xsl:attribute name="passCountPin"><xsl:value-of select="count(Component/Defect[@FailureName = 'NG' and @DefectName='PASS'])"/></xsl:attribute>
								<xsl:attribute name="errorCountPin"><xsl:value-of select="count(Component/Defect[@FailureName = 'NG' and @DefectName='NG'])"/></xsl:attribute>
							</xsl:element>
						</xsl:for-each>
					</xsl:element>
					
				</xsl:element>
			</xsl:element>
		</xsl:element> <!-- body - end -->	
	</xsl:element> <!-- root - end -->	
</xsl:template>

</xsl:stylesheet>
