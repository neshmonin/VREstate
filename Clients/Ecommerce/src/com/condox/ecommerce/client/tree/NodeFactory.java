package com.condox.ecommerce.client.tree;

import com.condox.clientshared.tree.TreeNode;
import com.condox.ecommerce.client.tree.model.BuildingsModel;
import com.condox.ecommerce.client.tree.model.EmailModel;
import com.condox.ecommerce.client.tree.model.ListingOptionsModel;
import com.condox.ecommerce.client.tree.model.LoginModel;
import com.condox.ecommerce.client.tree.model.MLSModel;
import com.condox.ecommerce.client.tree.model.ProductModel;
import com.condox.ecommerce.client.tree.model.SuitesModel;
import com.condox.ecommerce.client.tree.model.SummaryModel;

public class NodeFactory {
	public static TreeNode create(String type) {
		if (DefaultNode.simpleName.equals(type))
			return new DefaultNode();
		if (BuildingsModel.simpleName.equals(type))
			return new BuildingsModel();
		if (EmailModel.simpleName.equals(type))
			return new EmailModel();
		if (ListingOptionsModel.simpleName.equals(type))
			return new ListingOptionsModel();
		if (LoginModel.simpleName.equals(type))
			return new LoginModel();
		if (MLSModel.simpleName.equals(type))
			return new MLSModel();
		if (ProductModel.simpleName.equals(type))
			return new ProductModel();
		if (SuitesModel.simpleName.equals(type))
			return new SuitesModel();
		if (SummaryModel.simpleName.equals(type))
			return new SummaryModel();
		
		return null;
	} 
}
