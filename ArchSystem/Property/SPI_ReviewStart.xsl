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
            <xsl:text>ReviewStart</xsl:text>
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
            <xsl:value-of select="$delimiter"/>
            <!-- ReviewUser -->
            <xsl:text>"ReviewUser":"</xsl:text>
            <xsl:value-of select="InspectResult/PCB/Array/Panel/Component/Pad[@DefectUserName!='']/@DefectUserName"/>
            <xsl:text>"</xsl:text>
        <xsl:text>}</xsl:text>
        <xsl:value-of select="$delimiter"/>

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
            <xsl:value-of select="InspectResult/PCB/@ResultCustomerName"/>
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
                        <xsl:value-of select="@PanelResultCustomerName"/>
                        <xsl:text>"</xsl:text>
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
