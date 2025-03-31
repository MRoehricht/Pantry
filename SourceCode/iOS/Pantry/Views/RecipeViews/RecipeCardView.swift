//
//  Untitled.swift
//  Pantry
//
//  Created by Matthias Röhricht on 30.03.25.
//

import SwiftUI

struct RecipeCardView: View {
    var recipe: Recipe

    var body: some View {
        VStack(alignment: .leading) {
            if let imageData = recipe.imageData, let uiImage = UIImage(data: imageData) {
                Image(uiImage: uiImage)
                    .resizable()
                    .scaledToFill()
                    .frame(height: 200)
                    .clipped()
            } else {
                Image("dummy_recipe")
                    .resizable()
                    .scaledToFill()
                    .frame(height: 200)
                    .clipped()
                    .opacity(0.7)
            }
            
            Text(recipe.name)
                .font(.headline)
                .padding([.leading, .bottom, .trailing])
        }
        .background(Color.white)
        .cornerRadius(10)
        .shadow(radius: 5)
        .padding([.top, .horizontal])
    }
}

#Preview {
    RecipeCardView(recipe: Recipe(name: "Test", ingredients: []))
}
