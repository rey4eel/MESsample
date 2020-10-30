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
        public string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    ]]>
    </ms:script>

    <!-- variables -->
    <xsl:variable name="delimiter" select="','"/>

    <!-- params -->
    <xsl:param name="WebServiceContents"/>

<xsl:template match="/">

    <xsl:text>{</xsl:text>

        <!-- Event -->
        <xsl:text>"Event":{</xsl:text>
            <!-- Id -->
            <xsl:text>"Id":"</xsl:text>
            <xsl:value-of select="cs:GetGuid()"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Name -->
            <xsl:text>"Name":"</xsl:text>
            <xsl:text>InspectEnd</xsl:text>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Timestamp -->
            <xsl:text>"Timestamp":"</xsl:text>
            <xsl:value-of select="extobj:DateTime_UtcNow('yyyy-MM-ddTHH:mm:ss')"/>
            <xsl:text>"</xsl:text>
        <xsl:text>}</xsl:text>
        <xsl:value-of select="$delimiter"/>

        <!-- Info -->
        <xsl:text>"Info":{</xsl:text>
            <!-- Machine -->
            <xsl:text>"Machine":"</xsl:text>
            <xsl:value-of select="extobj:SPI_Config('SPI', 'HW', 'MACHINE_NAME')"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Lane -->
            <xsl:text>"Lane":"</xsl:text>
            <xsl:value-of select="number(InspectResult/PCB/@Lane) + 1"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Program -->
            <xsl:text>"Program":"</xsl:text>
            <xsl:value-of select="InspectResult/PCB/@JobFileName"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Model -->
            <xsl:text>"Model":"</xsl:text>
            <xsl:value-of select="InspectResult/PCB/@PcbName"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Side -->
            <xsl:text>"Side":"</xsl:text>
            <xsl:choose>
                <xsl:when test="InspectResult/PCB/@Side=0">
                    <xsl:text>Top</xsl:text>
                </xsl:when>
                <xsl:when test="InspectResult/PCB/@Side=1">
                    <xsl:text>Bottom</xsl:text>
                </xsl:when>
            </xsl:choose>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Lot -->
            <xsl:text>"Lot":"</xsl:text>
            <xsl:value-of select="InspectResult/PCB/@Lot"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- User -->
            <xsl:text>"User":"</xsl:text>
            <xsl:value-of select="InspectResult/PCB/@User"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- InspectStart -->
            <xsl:text>"InspectStart":"</xsl:text>
            <xsl:value-of select="InspectResult/PCB/@InspectStartDateTime"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- InspectEnd -->
            <xsl:text>"InspectEnd":"</xsl:text>
            <xsl:value-of select="InspectResult/PCB/@InspectEndDateTime"/>
            <xsl:text>"</xsl:text>
        <xsl:text>}</xsl:text>
        <xsl:value-of select="$delimiter"/>

        <!-- Count -->
        <xsl:if test="contains($WebServiceContents, 'Count')">
            <xsl:text>"Count":{</xsl:text>
                <!-- LotCount -->
                <xsl:text>"LotCount":</xsl:text>
                <xsl:value-of select="InspectResult/PCB/@LotCnt"/>
                <xsl:value-of select="$delimiter"/>
                <!-- StencilCount -->
                <xsl:text>"StencilCount":</xsl:text>
                <xsl:value-of select="InspectResult/PCB/@StencilCount"/>
                <xsl:value-of select="$delimiter"/>
                <!-- InspectTime -->
                <xsl:text>"InspectTime":</xsl:text>
                <xsl:value-of select="translate(format-number(InspectResult/PCB/@TestTime, '0.###'), 'aN', '0')"/>
                <xsl:value-of select="$delimiter"/>
                <!-- ReviewTime -->
                <xsl:text>"ReviewTime":</xsl:text>
                <xsl:value-of select="translate(format-number(InspectResult/PCB/@DefectTime div 1000, '0.###'), 'aN', '0')"/>
                <xsl:value-of select="$delimiter"/>
                <!-- TotalPad -->
                <xsl:text>"TotalPad":</xsl:text>
                <xsl:value-of select="count(InspectResult/PCB/Array/Panel/Component/Pad)"/>
                <xsl:value-of select="$delimiter"/>
                <!-- GoodPad -->
                <xsl:text>"GoodPad":</xsl:text>
                <xsl:value-of select="count(InspectResult/PCB/Array/Panel/Component/Pad[@Result=1 or (@Result&lt;-10 and @Result&gt;-100)])"/>
                <xsl:value-of select="$delimiter"/>
                <!-- PassPad -->
                <xsl:text>"PassPad":</xsl:text>
                <xsl:value-of select="count(InspectResult/PCB/Array/Panel/Component/Pad[@DefectResult=2])"/>
                <xsl:value-of select="$delimiter"/>
                <!-- BadPad -->
                <xsl:text>"BadPad":</xsl:text>
                <xsl:value-of select="count(InspectResult/PCB/Array/Panel/Component/Pad[@DefectResult&lt;-100])"/>
            <xsl:text>}</xsl:text>
            <xsl:value-of select="$delimiter"/>
        </xsl:if>

        <!-- PCB -->
        <xsl:text>"PCB":{</xsl:text>
            <!-- Array -->
            <xsl:text>"Array":"</xsl:text>
            <xsl:value-of select="InspectResult/PCB/@PanelCount"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Barcode -->
            <xsl:text>"Barcode":"</xsl:text>
            <xsl:value-of select="InspectResult/PCB/@Barcode"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Result -->
            <xsl:text>"Result":"</xsl:text>
            <xsl:value-of select="InspectResult/PCB/@DefectResultCustomerName"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Panel -->
            <xsl:text>"Panel":[</xsl:text>
                <xsl:for-each select="InspectResult/PCB/Array/Panel">
                    <xsl:sort select="@PanelIndex" data-type="number"/>
                    <xsl:text>{</xsl:text>
                        <!-- Array -->
                        <xsl:text>"Array":"</xsl:text>
                        <xsl:value-of select="@PanelIndex"/>
                        <xsl:text>"</xsl:text>
                        <xsl:value-of select="$delimiter"/>
                        <!-- Barcode -->
                        <xsl:text>"Barcode":"</xsl:text>
                        <xsl:value-of select="@PanelBarcode"/>
                        <xsl:text>"</xsl:text>
                        <xsl:value-of select="$delimiter"/>
                        <!-- Result -->
                        <xsl:text>"Result":"</xsl:text>
                        <xsl:value-of select="@PanelDefectResultCustomerName"/>
                        <xsl:text>"</xsl:text>
                        <!-- Defect -->
                        <xsl:if test="contains($WebServiceContents, 'Defect')">
                            <xsl:if test="@PanelResultName!='GOOD'">
                                <xsl:value-of select="$delimiter"/>
                                <xsl:text>"Defect":[</xsl:text>
                                    <xsl:for-each select="Component/Pad[@Result&lt;-100]">
                                        <xsl:sort select="@PadId" data-type="number"/>
                                        <xsl:text>{</xsl:text>
                                            <!-- Pad -->
                                            <xsl:text>"Pad":"</xsl:text>
                                            <xsl:value-of select="@PadId"/>
                                            <xsl:text>"</xsl:text>
                                            <xsl:value-of select="$delimiter"/>
                                            <!-- Component -->
                                            <xsl:text>"Component":"</xsl:text>
                                            <xsl:value-of select="../@Name"/>
                                            <xsl:text>"</xsl:text>
                                            <xsl:value-of select="$delimiter"/>
                                            <!-- Part -->
                                            <xsl:text>"Part":"</xsl:text>
                                            <xsl:value-of select="@LibName"/>
                                            <xsl:text>"</xsl:text>
                                            <xsl:value-of select="$delimiter"/>
                                            <!-- Pin -->
                                            <xsl:text>"Pin":"</xsl:text>
                                            <xsl:value-of select="@Pin"/>
                                            <xsl:text>"</xsl:text>
                                            <xsl:value-of select="$delimiter"/>
                                            <!-- Type -->
                                            <xsl:text>"Type":"</xsl:text>
                                            <xsl:value-of select="@ResultCustomerName"/>
                                            <xsl:text>"</xsl:text>
                                        <xsl:text>}</xsl:text>
                                        <xsl:if test="position()!=last()">
                                            <xsl:value-of select="$delimiter"/>
                                        </xsl:if>
                                    </xsl:for-each>
                                <xsl:text>]</xsl:text>
                            </xsl:if>
                        </xsl:if>
                        <!-- Condition -->
                        <xsl:if test="contains($WebServiceContents, 'Condition')">
                            <xsl:value-of select="$delimiter"/>
                            <xsl:text>"Condition":[</xsl:text>
                                <xsl:for-each select="Component/Pad">
                                    <xsl:sort select="@PadId" data-type="number"/>
                                    <xsl:text>{</xsl:text>
                                        <!-- Pad -->
                                        <xsl:text>"Pad":"</xsl:text>
                                        <xsl:value-of select="@PadId"/>
                                        <xsl:text>"</xsl:text>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- VolumeMax -->
                                        <xsl:text>"VolumeMax":</xsl:text>
                                        <xsl:choose>
                                            <xsl:when test="@COND_CheckVolume=true()">
                                                <xsl:value-of select="translate(format-number(@COND_Volume_MAX, '0.###'), 'aN', '0')"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:text>null</xsl:text>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- VolumeMin -->
                                        <xsl:text>"VolumeMin":</xsl:text>
                                        <xsl:choose>
                                            <xsl:when test="@COND_CheckVolume=true()">
                                                <xsl:value-of select="translate(format-number(@COND_Volume_MIN, '0.###'), 'aN', '0')"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:text>null</xsl:text>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- AreaMax -->
                                        <xsl:text>"AreaMax":</xsl:text>
                                        <xsl:choose>
                                            <xsl:when test="@COND_CheckArea=true()">
                                                <xsl:value-of select="translate(format-number(@COND_Area_MAX, '0.###'), 'aN', '0')"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:text>null</xsl:text>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- AreaMin -->
                                        <xsl:text>"AreaMin":</xsl:text>
                                        <xsl:choose>
                                            <xsl:when test="@COND_CheckArea=true()">
                                                <xsl:value-of select="translate(format-number(@COND_Area_MIN, '0.###'), 'aN', '0')"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:text>null</xsl:text>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- HeightMax -->
                                        <xsl:text>"HeightMax":</xsl:text>
                                        <xsl:choose>
                                            <xsl:when test="@COND_CheckHeight=true()">
                                                <xsl:value-of select="translate(format-number(@COND_Height_Upper, '0.###'), 'aN', '0')"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:text>null</xsl:text>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- HeightMin -->
                                        <xsl:text>"HeightMin":</xsl:text>
                                        <xsl:choose>
                                            <xsl:when test="@COND_CheckHeight=true()">
                                                <xsl:value-of select="translate(format-number(@COND_Height_Lower, '0.###'), 'aN', '0')"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:text>null</xsl:text>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- OffsetXMax -->
                                        <xsl:text>"OffsetXMax":</xsl:text>
                                        <xsl:choose>
                                            <xsl:when test="@COND_CheckOffsetX=true()">
                                                <xsl:value-of select="translate(format-number(@COND_OffsetX, '0.###'), 'aN', '0')"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:text>null</xsl:text>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- OffsetYMax -->
                                        <xsl:text>"OffsetYMax":</xsl:text>
                                        <xsl:choose>
                                            <xsl:when test="@COND_CheckOffsetY=true()">
                                                <xsl:value-of select="translate(format-number(@COND_OffsetY, '0.###'), 'aN', '0')"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:text>null</xsl:text>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                    <xsl:text>}</xsl:text>
                                    <xsl:if test="position()!=last()">
                                        <xsl:value-of select="$delimiter"/>
                                    </xsl:if>
                                </xsl:for-each>
                            <xsl:text>]</xsl:text>
                        </xsl:if>
                        <!-- Measurement -->
                        <xsl:if test="contains($WebServiceContents, 'Measurement')">
                            <xsl:value-of select="$delimiter"/>
                            <xsl:text>"Measurement":[</xsl:text>
                                <xsl:for-each select="Component/Pad">
                                    <xsl:sort select="@PadId" data-type="number"/>
                                    <xsl:text>{</xsl:text>
                                        <!-- Pad -->
                                        <xsl:text>"Pad":</xsl:text>
                                        <xsl:value-of select="@PadId"/>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- Volume -->
                                        <xsl:text>"Volume":</xsl:text>
                                        <xsl:value-of select="translate(format-number(@Volume, '0.###'), 'aN', '0')"/>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- Area -->
                                        <xsl:text>"Area":</xsl:text>
                                        <xsl:value-of select="translate(format-number(@Area, '0.###'), 'aN', '0')"/>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- Height -->
                                        <xsl:text>"Height":</xsl:text>
                                        <xsl:value-of select="translate(format-number(@Height, '0.###'), 'aN', '0')"/>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- OffsetX -->
                                        <xsl:text>"OffsetX":</xsl:text>
                                        <xsl:value-of select="translate(format-number(@OffsetX, '0.###'), 'aN', '0')"/>
                                        <xsl:value-of select="$delimiter"/>
                                        <!-- OffsetY -->
                                        <xsl:text>"OffsetY":</xsl:text>
                                        <xsl:value-of select="translate(format-number(@OffsetY, '0.###'), 'aN', '0')"/>
                                    <xsl:text>}</xsl:text>
                                    <xsl:if test="position()!=last()">
                                        <xsl:value-of select="$delimiter"/>
                                    </xsl:if>
                                </xsl:for-each>
                            <xsl:text>]</xsl:text>
                        </xsl:if>
                    <xsl:text>}</xsl:text>
                    <xsl:if test="position()!=last()">
                        <xsl:value-of select="$delimiter"/>
                    </xsl:if>
                </xsl:for-each>
            <xsl:text>]</xsl:text>
        <xsl:text>}</xsl:text>

    <xsl:text>}</xsl:text>

</xsl:template>

</xsl:stylesheet>
