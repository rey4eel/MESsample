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
        public string GetPanelBarcode(string barcode, int index)
        {
            var panelBarcode = barcode.Split(',');

            if (index > 0 && panelBarcode.Length >= index)
            {
                barcode = panelBarcode[index - 1];
            }
            else
            {
                barcode = string.Empty;
            }

            return barcode;
        }
    ]]>
    </ms:script>

    <!-- templates -->
    <xsl:template name="panel">
        <xsl:param name="count" select="0"/>
        <xsl:param name="index" select="1"/>

        <xsl:if test="$count!='' and $count!='0'">
            <xsl:text>{</xsl:text>
                <!-- Array -->
                <xsl:text>"Array":"</xsl:text>
                <xsl:value-of select="$index"/>
                <xsl:text>"</xsl:text>
                <xsl:value-of select="$delimiter"/>
                <!-- Barcode -->
                <xsl:text>"Barcode":"</xsl:text>
                <xsl:value-of select="cs:GetPanelBarcode($PanelBarcode, $index)"/>
                <xsl:text>"</xsl:text>
            <xsl:text>}</xsl:text>

            <xsl:if test="$index!=$count">
                <xsl:value-of select="$delimiter"/>
                <xsl:call-template name="panel">
                    <xsl:with-param name="count" select="$PanelCount"/>
                    <xsl:with-param name="index" select="$index+1"/>
                </xsl:call-template>
            </xsl:if>
        </xsl:if>
    </xsl:template>

    <!-- variables -->
    <xsl:variable name="delimiter" select="','"/>

    <!-- params -->
    <xsl:param name="Id"/>
    <xsl:param name="Machine"/>
    <xsl:param name="Lane"/>
    <xsl:param name="Program"/>
    <xsl:param name="Model"/>
    <xsl:param name="Side"/>
    <xsl:param name="Lot"/>
    <xsl:param name="User"/>
    <xsl:param name="PanelCount"/>
    <xsl:param name="PCBBarcode"/>
    <xsl:param name="PanelBarcode"/>

<xsl:template match="/">

    <xsl:text>{</xsl:text>

        <!-- Event -->
        <xsl:text>"Event":{</xsl:text>
            <!-- Id -->
            <xsl:text>"Id":"</xsl:text>
            <xsl:value-of select="$Id"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Name -->
            <xsl:text>"Name":"</xsl:text>
            <xsl:text>InspectStart</xsl:text>
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
            <xsl:value-of select="$Machine"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Lane -->
            <xsl:text>"Lane":"</xsl:text>
            <xsl:value-of select="number($Lane) + 1"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Program -->
            <xsl:text>"Program":"</xsl:text>
            <xsl:value-of select="$Program"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Model -->
            <xsl:text>"Model":"</xsl:text>
            <xsl:value-of select="$Model"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Side -->
            <xsl:text>"Side":"</xsl:text>
            <xsl:choose>
                <xsl:when test="$Side=0">
                    <xsl:text>Top</xsl:text>
                </xsl:when>
                <xsl:when test="$Side=1">
                    <xsl:text>Bottom</xsl:text>
                </xsl:when>
            </xsl:choose>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Lot -->
            <xsl:text>"Lot":"</xsl:text>
            <xsl:value-of select="$Lot"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- User -->
            <xsl:text>"User":"</xsl:text>
            <xsl:value-of select="$User"/>
            <xsl:text>"</xsl:text>
        <xsl:text>}</xsl:text>
        <xsl:value-of select="$delimiter"/>

        <!-- PCB -->
        <xsl:text>"PCB":{</xsl:text>
            <!-- Array -->
            <xsl:text>"Array":"</xsl:text>
            <xsl:value-of select="$PanelCount"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Barcode -->
            <xsl:text>"Barcode":"</xsl:text>
            <xsl:value-of select="$PCBBarcode"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Panel -->
            <xsl:text>"Panel":[</xsl:text>
                <xsl:call-template name="panel">
                    <xsl:with-param name="count" select="$PanelCount"/>
                </xsl:call-template>
            <xsl:text>]</xsl:text>
        <xsl:text>}</xsl:text>

    <xsl:text>}</xsl:text>

</xsl:template>

</xsl:stylesheet>
