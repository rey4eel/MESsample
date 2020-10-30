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
    <xsl:param name="StatusCode"/>
    <xsl:param name="StatusName"/>
    <xsl:param name="AlarmCode"/>
    <xsl:param name="AlarmName"/>

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
            <xsl:text>StatusChange</xsl:text>
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

        <!-- Status -->
        <xsl:text>"Status":{</xsl:text>
            <!-- Code -->
            <xsl:text>"Code":"</xsl:text>
            <xsl:value-of select="$StatusCode"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Name -->
            <xsl:text>"Name":"</xsl:text>
            <xsl:value-of select="$StatusName"/>
            <xsl:text>"</xsl:text>
            <xsl:value-of select="$delimiter"/>
            <!-- Alarm -->
            <xsl:text>"Alarm":[</xsl:text>
                <xsl:if test="$AlarmCode!='' and $AlarmCode!='0'">
                    <xsl:text>{</xsl:text>
                        <!-- Code -->
                        <xsl:text>"Code":"</xsl:text>
                        <xsl:value-of select="$AlarmCode"/>
                        <xsl:text>"</xsl:text>
                        <xsl:value-of select="$delimiter"/>
                        <!-- Name -->
                        <xsl:text>"Name":"</xsl:text>
                        <xsl:value-of select="$AlarmName"/>
                        <xsl:text>"</xsl:text>
                    <xsl:text>}</xsl:text>
                </xsl:if>
            <xsl:text>]</xsl:text>
        <xsl:text>}</xsl:text>

    <xsl:text>}</xsl:text>

</xsl:template>

</xsl:stylesheet>
