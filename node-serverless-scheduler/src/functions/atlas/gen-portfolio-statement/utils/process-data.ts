export const processData = async (data: any) => {
  const custcodeMarketingMapping = [];

  // Loop through each custcode in consolidatedSummary
  data.consolidatedSummary.custcode.forEach((custcode) => {
    // Check each marketing's portfoliosummary for the current custcode
    data.marketings.forEach((marketing) => {
      if (marketing.portfoliosummary.custcode.includes(custcode)) {
        // Add the marketing details and custcodes array to the custcodeMarketingMapping array
        custcodeMarketingMapping.push({
          custcode: custcode,
          marketingId: marketing.marketingId,
          marketingName: marketing.marketingName,
          marketingPhoneNo: marketing.marketingPhoneNo,
          custcodes: marketing.portfoliosummary.custcode, // Add the array of custcodes
        });
      }
    });
  });

  const extractMarketingNamesAndCustCodes = (marketings) => {
    return marketings.map((marketing) => {
      return {
        marketingName: marketing.marketingName,
        marketingPhoneNo: marketing.marketingPhoneNo,
        custcodes: marketing.portfoliosummary.custcode,
      };
    });
  };

  const calculateUnrealizedPL = (gainLoss, totalCost) => {
    if (!gainLoss || !totalCost || totalCost === 0) {
      return 0;
    }
    return (gainLoss / totalCost) * 100;
  };

  const getGroupedThaiEquityData = (thaiEquity, allocationAmount) => {
    const groupedData = thaiEquity.reduce((acc, item) => {
      if (!acc[item.custcode]) {
        acc[item.custcode] = {};
      }
      if (!acc[item.custcode][item.accountName]) {
        acc[item.custcode][item.accountName] = {
          accountName: item.accountName,
          accountNo: item.accountNo,
          custcode: item.custcode,
          dateKey: item.dateKey,
          totalMarketValue: 0,
          totalGainLoss: 0,
          totalCost: 0,
          products: [],
        };
      }
      item.unrealizedPL = calculateUnrealizedPL(
        parseFloat(item.gainLoss),
        parseFloat(item.totalCost)
      );
      acc[item.custcode][item.accountName].totalMarketValue += item.marketValue
        ? parseFloat(item.marketValue)
        : 0;
      acc[item.custcode][item.accountName].totalGainLoss += item.gainLoss
        ? parseFloat(item.gainLoss)
        : 0;
      acc[item.custcode][item.accountName].totalCost += item.totalCost
        ? parseFloat(item.totalCost)
        : 0;
      acc[item.custcode][item.accountName].products.push(item);
      return acc;
    }, {});

    const result = [];
    Object.keys(groupedData).forEach((custcode) => {
      Object.keys(groupedData[custcode]).forEach((accountName) => {
        const accountData = groupedData[custcode][accountName];

        // Sort the products array by sharecode
        accountData.products.sort((a, b) =>
          a.sharecode.localeCompare(b.sharecode)
        );

        groupedData[custcode][accountName].unrealizedPL = calculateUnrealizedPL(
          groupedData[custcode][accountName].totalGainLoss,
          groupedData[custcode][accountName].totalCost
        );
        groupedData[custcode][accountName].totalPortfolioAllocation =
          allocationAmount != 0
            ? (accountData.totalMarketValue / allocationAmount) * 100
            : 0.0001;

        result.push(groupedData[custcode][accountName]);
      });
    });

    return result;
  };

  const getGroupedMutualFundData = (mutualFund, allocationAmount) => {
    const groupedData = mutualFund.reduce((acc, item) => {
      if (!acc[item.custcode]) {
        acc[item.custcode] = {};
      }
      if (!acc[item.custcode][item.fundCategory]) {
        acc[item.custcode][item.fundCategory] = {
          fundCategory: item.fundCategory,
          accountNo: item.accountNo,
          custcode: item.custcode,
          dateKey: item.dateKey,
          totalMarketValue: 0,
          totalGainLoss: 0,
          totalCost: 0,
          products: [],
        };
      }
      item.unrealizedPL = calculateUnrealizedPL(
        parseFloat(item.gainLoss),
        parseFloat(item.totalCost)
      );
      acc[item.custcode][item.fundCategory].totalMarketValue += parseFloat(
        item.marketValue
      );
      acc[item.custcode][item.fundCategory].totalGainLoss += parseFloat(
        item.gainLoss
      );
      acc[item.custcode][item.fundCategory].totalCost += parseFloat(
        item.totalCost
      );
      acc[item.custcode][item.fundCategory].products.push(item);
      return acc;
    }, {});

    // Convert the grouped data into an array format
    const result = [];
    Object.keys(groupedData).forEach((custcode) => {
      const categories = groupedData[custcode];
      Object.keys(categories).forEach((fundCategory) => {
        const category = categories[fundCategory];

        // Sort the products array by fundName
        category.products.sort((a, b) => a.fundName.localeCompare(b.fundName));

        const totalMarketValue = Object.values(categories).reduce(
          (acc, cat) => acc + cat.totalMarketValue,
          0
        );
        category.percentAllocation =
          (category.totalMarketValue / totalMarketValue) * 100;
        category.totalPortfolioAllocation =
          allocationAmount != 0
            ? (category.totalMarketValue / allocationAmount) * 100
            : 0.0001;
        result.push(category);
      });
    });

    return result;
  };

  const getGroupedBondData = (bond) => {
    const groupedData = bond.reduce((acc, item) => {
      if (!acc[item.custcode]) {
        acc[item.custcode] = {};
      }
      if (!acc[item.custcode][item.marketType]) {
        acc[item.custcode][item.marketType] = {
          marketType: item.marketType,
          accountNo: item.accountNo,
          custcode: item.custcode,
          dateKey: item.dateKey,
          totalMarketValue: 0,
          totalCost: 0,
          products: [],
        };
      }
      acc[item.custcode][item.marketType].totalMarketValue += parseFloat(
        item.marketValue
      );
      acc[item.custcode][item.marketType].totalGainLoss += parseFloat(
        item.gainLoss
      );
      acc[item.custcode][item.marketType].totalCost += parseFloat(
        item.totalCost
      );
      acc[item.custcode][item.marketType].products.push(item);
      return acc;
    }, {});

    // const totalMarketValue = Object.values(groupedData).reduce((acc, marketType) => acc + marketType.totalMarketValue, 0);

    const result = [];
    Object.keys(groupedData).forEach((custcode) => {
      const categories = groupedData[custcode];
      Object.keys(categories).forEach((marketType) => {
        const category = categories[marketType];

        category.products.sort((a, b) =>
          a.assetName.localeCompare(b.assetName)
        );

        const totalMarketValue = Object.values(categories).reduce(
          (acc, cat) => acc + cat.totalMarketValue,
          0
        );
        category.percentAllocation =
          (category.totalMarketValue / totalMarketValue) * 100;

        result.push(category);
      });
    });

    return result;
  };

  const getGroupedTFEXData = (tfex, totalAccountMarketValue) => {
    const groupedData = tfex.reduce((acc, item) => {
      if (!acc[item.custcode]) {
        acc[item.custcode] = {
          accountNo: item.accountNo,
          custcode: item.custcode,
          dateKey: item.dateKey,
          accountName: item.accountName,
          totalMarketValue: 0,
          totalGainLoss: 0,
          totalCost: 0,
          products: [],
        };
      }
      item.unrealizedPL = calculateUnrealizedPL(
        parseFloat(item.gainLoss),
        parseFloat(item.totalCost)
      );
      acc[item.custcode].totalMarketValue += item.marketValue
        ? parseFloat(item.marketValue)
        : 0;
      acc[item.custcode].totalGainLoss += item.gainLoss
        ? parseFloat(item.gainLoss)
        : 0;
      acc[item.custcode].totalCost += parseFloat(item.totalCost);
      acc[item.custcode].products.push(item);
      return acc;
    }, {});

    Object.values(groupedData).forEach((account) => {
      account.totalPortfolioAllocation =
        (account.totalMarketValue / totalAccountMarketValue) * 100;
      account.products.sort((a, b) => a.sharecode.localeCompare(b.sharecode));
    });

    return Object.values(groupedData);
  };

  const getGroupedCashData = (cash, allocationAmount) => {
    const groupedData = cash.reduce((acc, item) => {
      if (!acc[item.custcode]) {
        acc[item.custcode] = {};
      }
      if (!acc[item.custcode][item.currency]) {
        acc[item.custcode][item.currency] = {
          custcode: item.custcode,
          currency: item.currency,
          currencyFullName: item.currencyFullName,
          accountDetails: [],
        };
      }
      acc[item.custcode][item.currency].accountDetails.push({
        accountNo: item.accountNo,
        cashBalance: item.cashBalance,
        dateKey: item.dateKey,
        marketValue: parseFloat(item.marketValue),
        accountName: item.accountName,
      });
      return acc;
    }, {});

    // Combine objects with the same custcode and currency into a single array and calculate totalMarketValue and totalPortfolioAllocation
    return Object.keys(groupedData).map((custcode) => {
      const currencies = Object.values(groupedData[custcode]);
      currencies.sort((a, b) =>
        a.currencyFullName.localeCompare(b.currencyFullName)
      );
      currencies.forEach((currency) => {
        currency.totalMarketValue = currency.accountDetails.reduce(
          (acc, account) => acc + account.marketValue,
          0
        );
        currency.totalCashBalance = currency.accountDetails.reduce(
          (acc, account) => acc + parseFloat(account.cashBalance),
          0
        );
      });
      return {
        custcode,
        totalMarketValue: currencies.reduce(
          (acc, currency) => acc + currency.totalMarketValue,
          0
        ),
        totalPortfolioAllocation:
          allocationAmount != 0
            ? (currencies.reduce(
                (acc, currency) => acc + currency.totalMarketValue,
                0
              ) /
                allocationAmount) *
              100
            : 0.001,
        currencies,
      };
    });
  };

  const getGroupedStructuredProductData = (
    structuredProduct,
    allocationAmount
  ) => {
    const groupedData = structuredProduct.reduce((acc, item) => {
      if (!acc[item.custcode]) {
        acc[item.custcode] = {
          accountNo: item.accountNo,
          custcode: item.custcode,
          dateKey: item.dateKey,
          accountName: item.accountName,
          totalMarketValue: 0,
          products: [],
        };
      }
      acc[item.custcode].totalMarketValue += parseFloat(item.marketValue);
      acc[item.custcode].products.push(item);
      return acc;
    }, {});

    const totalMarketValue = Object.values(groupedData).reduce(
      (acc, custcode) => acc + custcode.totalMarketValue,
      0
    );

    Object.values(groupedData).forEach((custcode) => {
      custcode.products.sort((a, b) =>
        a.productType?.localeCompare(b.productType)
      );
      custcode.totalPortfolioAllocation =
        allocationAmount != 0
          ? (custcode.totalMarketValue / allocationAmount) * 100
          : 0.001;
    });

    return Object.values(groupedData);
  };

  const getGroupedGlobalEquityOTCData = (globalEquityOtc, allocationAmount) => {
    const groupedData = globalEquityOtc.reduce((acc, item) => {
      if (!acc[item.custcode]) {
        acc[item.custcode] = {};
      }
      if (!acc[item.custcode][item.equityCategory]) {
        acc[item.custcode][item.equityCategory] = {
          equityCategory: item.equityCategory,
          accountNo: item.accountNo,
          custcode: item.custcode,
          dateKey: item.dateKey,
          accountName: item.accountName,
          totalMarketValue: 0,
          totalMarketValueOriginalCurrency: 0,
          totalGainLoss: 0,
          totalCost: 0,
          products: [],
        };
      }
      item.unrealizedPL = calculateUnrealizedPL(
        parseFloat(item.gainLoss),
        parseFloat(item.totalCost)
      );
      acc[item.custcode][item.equityCategory].totalMarketValue += parseFloat(
        item.marketValue || 0
      );
      acc[item.custcode][
        item.equityCategory
      ].totalMarketValueOriginalCurrency += parseFloat(
        item.marketValueOriginalCurrency || 0
      );
      acc[item.custcode][item.equityCategory].totalCost += parseFloat(
        item.totalCost || 0
      );
      acc[item.custcode][item.equityCategory].totalGainLoss += parseFloat(
        item.gainLoss || 0
      );
      acc[item.custcode][item.equityCategory].products.push(item);
      return acc;
    }, {});

    const totalMarketValue = globalEquityOtc.reduce(
      (acc, item) => acc + parseFloat(item.marketValue),
      0
    );

    const result = [];
    Object.keys(groupedData).forEach((custcode) => {
      const categories = groupedData[custcode];
      Object.keys(categories).forEach((equityCategory) => {
        const categoryData = categories[equityCategory];
        categoryData.products.sort((a, b) =>
          a.sharecode.localeCompare(b.sharecode)
        );
        const totalMarketValue = Object.values(categories).reduce(
          (acc, cat) => acc + cat.totalMarketValue,
          0
        );
        categoryData.percentAllocation =
          (categoryData.totalMarketValue / totalMarketValue) * 100;
        categoryData.totalPortfolioAllocation =
          allocationAmount != 0
            ? (categoryData.totalMarketValue / allocationAmount) * 100
            : 0.0001;
        result.push(categoryData);
      });
    });
    return result;
  };

  const getGroupedGlobalEquityData = (globalEquity, allocationAmount) => {
    const groupedData = globalEquity.reduce((acc, item) => {
      if (!acc[item.custcode]) {
        acc[item.custcode] = {};
      }
      if (!acc[item.custcode][item.equityCategory]) {
        acc[item.custcode][item.equityCategory] = {
          equityCategory: item.equityCategory,
          accountNo: item.accountNo,
          custcode: item.custcode,
          dateKey: item.dateKey,
          accountName: item.accountName,
          totalMarketValue: 0,
          totalMarketValueOriginalCurrency: 0,
          totalGainLoss: 0,
          totalCost: 0,
          products: [],
        };
      }
      item.unrealizedPL = calculateUnrealizedPL(
        parseFloat(item.gainLoss),
        parseFloat(item.totalCost)
      );
      acc[item.custcode][item.equityCategory].totalMarketValue += parseFloat(
        item.marketValue || 0
      );
      acc[item.custcode][
        item.equityCategory
      ].totalMarketValueOriginalCurrency += parseFloat(
        item.marketValueOriginalCurrency || 0
      );
      acc[item.custcode][item.equityCategory].totalCost += parseFloat(
        item.totalCost || 0
      );
      acc[item.custcode][item.equityCategory].totalGainLoss += parseFloat(
        item.gainLoss || 0
      );
      acc[item.custcode][item.equityCategory].products.push(item);
      return acc;
    }, {});

    // const totalMarketValue = globalEquity.reduce((acc, item) => acc + parseFloat(item.marketValue), 0);

    const result = [];
    Object.keys(groupedData).forEach((custcode) => {
      const categories = groupedData[custcode];
      Object.keys(categories).forEach((equityCategory) => {
        const categoryData = categories[equityCategory];
        categoryData.products.sort((a, b) =>
          a.sharecode.localeCompare(b.sharecode)
        );
        const totalMarketValue = Object.values(categories).reduce(
          (acc, cat) => acc + cat.totalMarketValue,
          0
        );
        categoryData.percentAllocation =
          (categoryData.totalMarketValue / totalMarketValue) * 100;
        categoryData.totalPortfolioAllocation =
          allocationAmount != 0
            ? (categoryData.totalMarketValue / allocationAmount) * 100
            : 0.0001;
        result.push(categoryData);
      });
    });
    return result;
  };

  const getGroupedAlternativesPrivateEquityData = (
    alternativePrivateEquity,
    allocationAmount
  ) => {
    const groupedData = alternativePrivateEquity.reduce((acc, item) => {
      if (!acc[item.custcode]) {
        acc[item.custcode] = {};
      }
      if (!acc[item.custcode][item.category]) {
        acc[item.custcode][item.category] = {
          category: item.category,
          accountNo: item.accountNo,
          dateKey: item.dateKey,
          totalMarketValue: 0,
          totalGainLoss: 0,
          totalCost: 0,
          products: [],
        };
      }
      acc[item.custcode][item.category].totalMarketValue += parseFloat(
        item.marketValue
      );
      acc[item.custcode][item.category].totalCost += parseFloat(
        item.totalCost || 0
      );
      acc[item.custcode][item.category].totalGainLoss += parseFloat(
        item.gainLoss || 0
      );
      acc[item.custcode][item.category].products.push(item);
      return acc;
    }, {});

    const totalMarketValue = alternativePrivateEquity.reduce(
      (acc, item) => acc + parseFloat(item.marketValue),
      0
    );

    const result = [];
    Object.keys(groupedData).forEach((custcode) => {
      Object.keys(groupedData[custcode]).forEach((category) => {
        const categoryData = groupedData[custcode][category];
        categoryData.products.sort((a, b) =>
          a.sharecode.localeCompare(b.sharecode)
        );
        categoryData.totalPortfolioAllocation =
          allocationAmount != 0
            ? (categoryData.totalMarketValue / allocationAmount) * 100
            : 0.0001;
        result.push({
          custcode,
          accountNo: categoryData.accountNo,
          category: category,
          totalMarketValue: categoryData.totalMarketValue,
          totalGainLoss: categoryData.totalGainLoss,
          totalCost: categoryData.totalCost,
          totalPortfolioAllocation: categoryData.totalPortfolioAllocation,
          products: categoryData.products,
        });
      });
    });

    return result;
  };

  const getGroupedAlternativesPrivateCreditData = (
    alternativePrivateCredit,
    allocationAmount
  ) => {
    const groupedData = alternativePrivateCredit.reduce((acc, item) => {
      if (!acc[item.custcode]) {
        acc[item.custcode] = {};
      }
      if (!acc[item.custcode][item.category]) {
        acc[item.custcode][item.category] = {
          totalMarketValue: 0,
          totalCost: 0,
          products: [],
        };
      }
      acc[item.custcode][item.category].totalMarketValue += parseFloat(
        item.marketValue
      );
      acc[item.custcode][item.category].totalCost += parseFloat(
        item.initialValue || 0
      );
      acc[item.custcode][item.category].products.push(item);
      return acc;
    }, {});

    const totalMarketValue = alternativePrivateCredit.reduce(
      (acc, item) => acc + parseFloat(item.marketValue),
      0
    );
    const totalCost = alternativePrivateCredit.reduce(
      (acc, item) => acc + parseFloat(item.initialValue),
      0
    );

    const result = [];
    Object.keys(groupedData).forEach((custcode) => {
      Object.keys(groupedData[custcode]).forEach((category) => {
        const categoryData = groupedData[custcode][category];
        categoryData.products.sort((a, b) =>
          a.assetName.localeCompare(b.assetName)
        );
        result.push({
          custcode,
          totalMarketValue: categoryData.totalMarketValue,
          totalCost: categoryData.totalCost,
          products: categoryData.products,
        });
      });
    });

    return result;
  };

  const getGroupedPrivateFundData = (privateFund, totalAccountMarketValue) => {
    const groupedData = privateFund.reduce((acc, item) => {
      if (!acc[item.custcode]) {
        acc[item.custcode] = {};
      }
      if (!acc[item.custcode][item.accountName]) {
        acc[item.custcode][item.accountName] = {
          accountName: item.accountName,
          accountNo: item.accountNo,
          custcode: item.custcode,
          dateKey: item.dateKey,
          totalMarketValue: 0,
          totalGainLoss: 0,
          totalCost: 0,
          products: [],
        };
      }
      item.unrealizedPL = calculateUnrealizedPL(
        parseFloat(item.gainLoss),
        parseFloat(item.totalCost)
      );
      acc[item.custcode][item.accountName].totalMarketValue += item.marketValue
        ? parseFloat(item.marketValue)
        : 0;
      acc[item.custcode][item.accountName].totalGainLoss += item.gainLoss
        ? parseFloat(item.gainLoss)
        : 0;
      acc[item.custcode][item.accountName].totalCost += item.totalCost
        ? parseFloat(item.totalCost)
        : 0;
      acc[item.custcode][item.accountName].products.push(item);
      return acc;
    }, {});

    const result = [];
    Object.keys(groupedData).forEach((custcode) => {
      Object.keys(groupedData[custcode]).forEach((accountName) => {
        const accountData = groupedData[custcode][accountName];
        accountData.products.sort((a, b) =>
          a.tickercode.localeCompare(b.tickercode)
        );
        groupedData[custcode][accountName].unrealizedPL = calculateUnrealizedPL(
          groupedData[custcode][accountName].totalGainLoss,
          groupedData[custcode][accountName].totalCost
        );
        groupedData[custcode][accountName].totalPortfolioAllocation =
          totalAccountMarketValue != 0
            ? (accountData.totalMarketValue / totalAccountMarketValue) * 100
            : 0.001;
        result.push(groupedData[custcode][accountName]);
      });
    });

    return result;
  };

  const finalResult = {
    custcodeMarketingMapping: custcodeMarketingMapping,
    thaiId: data.thaiId,
    customerName: data.customerName,
    customerAddress: data.customerAddress,
    isHighNetWorth: data.isHighNetWorth,
    exchangeRate: data.exchangeRate,
    consolidatedSummary: data.consolidatedSummary,
    main: extractMarketingNamesAndCustCodes(data.marketings),
    marketings: data.marketings.map((marketing) => {
      const groupedThaiEquityData = getGroupedThaiEquityData(
        marketing.thaiEquity,
        marketing.portfoliosummary.allocationAmount
      );
      const groupedMutualFundData = getGroupedMutualFundData(
        marketing.mutualFund,
        marketing.portfoliosummary.allocationAmount
      );
      const groupedMutualFundOffshoreData = getGroupedMutualFundData(
        marketing.mutualFundOffshore,
        marketing.portfoliosummary.allocationAmount
      );
      const groupedBondData = getGroupedBondData(marketing.bond);
      const groupedBondOffshoreData = getGroupedBondData(
        marketing.bondOffshore
      );
      const groupedTFEXData = getGroupedTFEXData(
        marketing.tfex,
        marketing.portfoliosummary.allocationAmount
      );
      const groupedCashData = getGroupedCashData(
        marketing.cash,
        marketing.portfoliosummary.allocationAmount
      );
      const groupedStructuredProductData = getGroupedStructuredProductData(
        marketing.structuredProduct,
        marketing.portfoliosummary.allocationAmount
      );
      const groupedStructuredProductOnshoreData =
        getGroupedStructuredProductData(
          marketing.structuredProductOnshore,
          marketing.portfoliosummary.allocationAmount
        );
      const groupedGlobalEquityOTCData = getGroupedGlobalEquityOTCData(
        marketing.globalEquityOtc,
        marketing.portfoliosummary.allocationAmount
      );
      const groupedGlobalEquityData = getGroupedGlobalEquityData(
        marketing.globalEquity,
        marketing.portfoliosummary.allocationAmount
      );
      const groupedalternativePrivateEquityData =
        getGroupedAlternativesPrivateEquityData(
          marketing.alternativePrivateEquity,
          marketing.portfoliosummary.allocationAmount
        );
      const groupedalternativePrivateCreditData =
        getGroupedAlternativesPrivateCreditData(
          marketing.alternativePrivateCredit,
          marketing.portfoliosummary.allocationAmount
        );
      const groupedPrivateFundData = getGroupedPrivateFundData(
        marketing.privateFund,
        marketing.portfoliosummary.allocationAmount
      );
      return {
        marketingId: marketing.marketingId,
        marketingName: marketing.marketingName,
        marketingPhoneNo: marketing.marketingPhoneNo,
        portfoliosummary: marketing.portfoliosummary,
        custcodeProductMapping: marketing.custcodeProductMapping,
        balanceSummary: marketing.balanceSummary,
        privateFundSummary: marketing.privateFundSummary,
        thaiEquity: groupedThaiEquityData,
        mutualFund: groupedMutualFundData,
        mutualFundOffshore: groupedMutualFundOffshoreData,
        bond: groupedBondData,
        bondOffshore: groupedBondOffshoreData,
        tfex: groupedTFEXData,
        cash: groupedCashData,
        structuredProduct: groupedStructuredProductData,
        structuredProductOnshore: groupedStructuredProductOnshoreData,
        globalEquityOtc: groupedGlobalEquityOTCData,
        globalEquity: groupedGlobalEquityData,
        alternatives: {
          PrivateEquity: groupedalternativePrivateEquityData,
          PrivateCredit: groupedalternativePrivateCreditData,
        },
        privateFund: groupedPrivateFundData,
      };
    }),
  };

  //console.log(JSON.stringify(finalResult, null, 2));

  return finalResult;
};
